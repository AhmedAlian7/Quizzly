using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.ViewModels.Student;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Enums;
using Quizzly.DataAccess.Repositories.Interfaces;
using System.Text.Json;

namespace Quizzly.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = AppRoles.Student)]
    public class QuizController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Start(string token)
        {
            var quiz = await _uow.Quizzes.GetByAccessTokenAsync(token, includeRelations: true);
            if (quiz == null)
                return RedirectToAction("Link", "Access", new { area = "Student", token });

            var user = await _userManager.GetUserAsync(User);
            var student = (await _uow.Students.GetAllAsync("")).FirstOrDefault(s => s.UserId == user!.Id);
            if (student == null)
                return Forbid();

            // validate availability
            var now = DateTime.UtcNow;
            if (!quiz.IsPublished || (quiz.StartAt.HasValue && now < quiz.StartAt.Value.ToUniversalTime()) || (quiz.EndAt.HasValue && now > quiz.EndAt.Value.ToUniversalTime()))
            {
                TempData["ErrorMessage"] = "Quiz is not available.";
                return RedirectToAction("Link", "Access", new { area = "Student", token });
            }

            // attempts policy
            var attempts = quiz.QuizAttempts.Where(a => a.StudentId == student.Id && a.IsCompleted).Count();
            if (!quiz.AllowMultipleAttempts && attempts > 0)
            {
                TempData["ErrorMessage"] = "You have already completed this quiz.";
                return RedirectToAction("Link", "Access", new { area = "Student", token });
            }
            if (quiz.AllowMultipleAttempts && quiz.MaxAttempts.HasValue && attempts >= quiz.MaxAttempts.Value)
            {
                TempData["ErrorMessage"] = "You have reached the maximum number of attempts.";
                return RedirectToAction("Link", new { token });
            }

            // create attempt
            var attempt = new QuizAttempt
            {
                AttemptNumber = attempts + 1,
                StudentIdentifier = user!.Email!,
                StartedAt = DateTime.UtcNow,
                IsCompleted = false,
                IsAutoGraded = quiz.IsAutoGraded,
                IsPublished = false,
                IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0",
                QuizId = quiz.Id,
                StudentId = student.Id,
                MaxScore = quiz.Questions.Sum(q => q.Points)
            };
            await _uow.QuizAttempts.AddAsync(attempt);
            await _uow.SaveAsync();

            return RedirectToAction(nameof(Take), new { attemptId = attempt.Id, index = 0 });
        }

        [HttpGet]
        public async Task<IActionResult> Take(int attemptId, int? questionId = null, int index = 0)
        {
            var attempt = await _uow.QuizAttempts.GetByIdAsync(attemptId, includes: "Quiz,Answers,Answers.Question,Answers.Choice");
            if (attempt == null || attempt.IsCompleted)
                return NotFound();

            var quiz = await _uow.Quizzes.GetByIdAsync(attempt.QuizId, includes: "Questions,Questions.Choices");
            if (quiz == null)
                return NotFound();

            // Check quiz availability window
            var nowUtc = DateTime.UtcNow;
            if (quiz.StartAt.HasValue && nowUtc < quiz.StartAt.Value)
                return BadRequest("Quiz has not started yet.");
            if (quiz.EndAt.HasValue && nowUtc > quiz.EndAt.Value)
                return BadRequest("Quiz has expired.");

            // end time for the timer
            var endsAt = attempt.StartedAt.AddMinutes(quiz.DurationMintes);

            var orderedQuestions = quiz.Questions.OrderBy(q => q.OrderIndex).ToList();

            // determine current index by questionId if provided
            var resolvedIndex = Math.Clamp(index, 0, Math.Max(0, orderedQuestions.Count - 1));
            if (questionId.HasValue)
            {
                var idxById = orderedQuestions.FindIndex(q => q.Id == questionId.Value);
                if (idxById >= 0) resolvedIndex = idxById;
            }

            var vm = new QuizTakingViewModel
            {
                QuizId = quiz.Id,
                QuizTitle = quiz.Title,
                AttemptId = attempt.Id,
                DurationIsSeconds = quiz.DurationMintes * 60,
                StartedAtUtc = attempt.StartedAt,
                EndsAtUtc = endsAt,
                CurrentIndex = resolvedIndex,
                TotalQuestions = orderedQuestions.Count,
                Questions = orderedQuestions.Select(q => new QuizTakingViewModel.QuestionVm
                {
                    QuestionId = q.Id,
                    OrderIndex = q.OrderIndex,
                    Text = q.Text,
                    QuestionType = q.QuestionType,
                    IsRequired = q.IsRequired,
                    Points = q.Points,
                    Explanation = q.Explanation,
                    Choices = q.Choices.OrderBy(c => c.OrderIndex).Select(c => new QuizTakingViewModel.ChoiceVm
                    {
                        ChoiceId = c.Id,
                        Text = c.Text
                    }).ToList(),
                    ExistingTextAnswer = attempt.Answers.FirstOrDefault(a => a.QuestionId == q.Id)?.TextAnswer,
                    ExistingChoiceIds = attempt.Answers
                        .Where(a => a.QuestionId == q.Id && a.ChoiceId.HasValue)
                        .Select(a => a.ChoiceId!.Value)
                        .ToList()
                }).ToList()
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int attemptId, string? answersJson)
        {
            var attempt = await _uow.QuizAttempts.GetByIdAsync(attemptId, includes: "Quiz,Answers,Answers.Question,Answers.Choice");
            if (attempt == null) return NotFound();

            if (attempt.IsCompleted)
                return RedirectToAction(nameof(Result), new { attemptId = attempt.Id });

            // Save answers from client JSON if provided
            if (!string.IsNullOrWhiteSpace(answersJson))
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<List<ClientAnswerDto>>(answersJson) ?? new();

                    // delete existing answers for attempt
                    foreach (var existing in attempt.Answers.ToList())
                    {
                        await _uow.Answers.DeleteAsync(existing.Id);
                    }

                    foreach (var p in payload)
                    {
                        if (p == null) continue;
                        if (p.choiceId.HasValue)
                        {
                            await _uow.Answers.AddAsync(new Answer { QuizAttemptId = attempt.Id, QuestionId = p.questionId, ChoiceId = p.choiceId, IsCorrect = false, IsGraded = false });
                        }
                        else if (!string.IsNullOrWhiteSpace(p.textAnswer))
                        {
                            await _uow.Answers.AddAsync(new Answer { QuizAttemptId = attempt.Id, QuestionId = p.questionId, TextAnswer = p.textAnswer, IsCorrect = false, IsGraded = false });
                        }
                    }
                    await _uow.SaveAsync();
                    // reload attempt with new answers for grading
                    attempt = await _uow.QuizAttempts.GetByIdAsync(attemptId, includes: "Quiz,Answers,Answers.Question,Answers.Choice");
                }
                catch
                {
                    // ignore bad payload and continue
                }
            }

            // finalize
            attempt.FinishedAt = DateTime.UtcNow;
            attempt.IsCompleted = true;

            // auto-grade simple types
            decimal score = 0m;
            foreach (var q in attempt.Quiz.Questions)
            {
                if (q.QuestionType == QuestionType.MCQ || q.QuestionType == QuestionType.TrueFalse)
                {
                    var selected = attempt.Answers.Where(a => a.QuestionId == q.Id && a.ChoiceId.HasValue).Select(a => a.ChoiceId!.Value).ToList();
                    var correct = q.Choices.Where(c => c.IsCorrect).Select(c => c.Id).OrderBy(i => i).ToList();
                    var chosen = selected.OrderBy(i => i).ToList();
                    bool isCorrect = correct.SequenceEqual(chosen);
                    if (isCorrect)
                    {
                        score += q.Points;
                        foreach (var ans in attempt.Answers.Where(a => a.QuestionId == q.Id))
                        {
                            ans.IsCorrect = true;
                            ans.IsGraded = true;
                            ans.PointsAwarded = q.Points;
                            ans.GradedAt = DateTime.UtcNow;
                        }
                    }
                    else
                    {
                        foreach (var ans in attempt.Answers.Where(a => a.QuestionId == q.Id))
                        {
                            ans.IsCorrect = false;
                            ans.IsGraded = true;
                            ans.PointsAwarded = 0m;
                            ans.GradedAt = DateTime.UtcNow;
                        }
                    }
                }
            }

            attempt.Score = score;
            attempt.Percentage = attempt.MaxScore > 0 ? Math.Round((score / attempt.MaxScore) * 100m, 2) : 0;
            _uow.QuizAttempts.Update(attempt);
            await _uow.SaveAsync();

            return RedirectToAction(nameof(Result), new { attemptId = attempt.Id });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int attemptId)
        {
            var attempt = await _uow.QuizAttempts.GetByIdAsync(attemptId, includes: "Quiz,Answers,Answers.Question,Answers.Choice");
            if (attempt == null) return NotFound();

            var vm = new QuizResultViewModel
            {
                QuizId = attempt.QuizId,
                QuizTitle = attempt.Quiz.Title,
                AttemptId = attempt.Id,
                Score = attempt.Score,
                MaxScore = attempt.MaxScore,
                Percentage = attempt.Percentage,
                TimeTaken = (attempt.FinishedAt ?? DateTime.UtcNow) - attempt.StartedAt,
                IsAutoGraded = attempt.IsAutoGraded,
                ShowCorrectAnswers = attempt.Quiz.ShowCorrectAnswers,
                Passed = attempt.Quiz.PassingScore.HasValue ? (attempt.Score ?? 0) >= attempt.Quiz.PassingScore.Value : false,
                AwaitingManualGrading = attempt.Answers.Any(a => a.Question.QuestionType == QuestionType.Essay || a.Question.QuestionType == QuestionType.ShortAnswer)
            };

            vm.Questions = attempt.Quiz.Questions.OrderBy(q => q.OrderIndex).Select(q => new QuizResultViewModel.QuestionResultVm
            {
                QuestionId = q.Id,
                Text = q.Text,
                Points = q.Points,
                PointsAwarded = attempt.Answers.Where(a => a.QuestionId == q.Id).Select(a => a.PointsAwarded).FirstOrDefault(),
                IsCorrect = attempt.Answers.Where(a => a.QuestionId == q.Id).Select(a => a.IsCorrect).FirstOrDefault(),
                Explanation = q.Explanation,
                Choices = q.Choices.OrderBy(c => c.OrderIndex).Select(c => new QuizResultViewModel.ChoiceResultVm
                {
                    ChoiceId = c.Id,
                    Text = c.Text,
                    IsCorrect = c.IsCorrect
                }).ToList(),
                SelectedChoiceIds = attempt.Answers.Where(a => a.QuestionId == q.Id && a.ChoiceId.HasValue).Select(a => a.ChoiceId!.Value).ToList(),
                TextAnswer = attempt.Answers.FirstOrDefault(a => a.QuestionId == q.Id)?.TextAnswer
            }).ToList();

            return View("~/Areas/Student/Views/Quiz/Result.cshtml", vm);
        }
    }
}


