using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Student;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Enums;
using Quizzly.DataAccess.Repositories.Interfaces;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace Quizzly.Business.Services.Implementions
{
    public class StudentQuizService : IStudentQuizService
    {
        private readonly IUnitOfWork _uow;

        public StudentQuizService(IUnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<int> StartAttemptAsync(string token, string userId, string userEmail, string? ipAddress)
        {
            var quiz = await _uow.Quizzes.GetByAccessTokenAsync(token,
                includes: "Instructor.User,Questions,Questions.Choices,StudentInfoFields");
            if (quiz == null)
                throw new KeyNotFoundException("Quiz not found");

            var student = await _uow.Students.GetByUserIdAsync(userId);
            if (student == null) throw new UnauthorizedAccessException("Student not found");

            var now = DateTime.UtcNow;
            if (!quiz.IsPublished || (quiz.StartAt.HasValue && now < quiz.StartAt.Value.ToUniversalTime()) || (quiz.EndAt.HasValue && now > quiz.EndAt.Value.ToUniversalTime()))
            {
                throw new InvalidOperationException("Quiz is not available.");
            }

            var attempts = await _uow.QuizAttempts.CountCompletedAttemptsForStudentAsync(quiz.Id, student.Id);
            if (!quiz.AllowMultipleAttempts && attempts > 0)
                throw new InvalidOperationException("You have already completed this quiz.");
            if (quiz.AllowMultipleAttempts && quiz.MaxAttempts.HasValue && attempts >= quiz.MaxAttempts.Value)
                throw new InvalidOperationException("You have reached the maximum number of attempts.");

            var attempt = new QuizAttempt
            {
                AttemptNumber = attempts + 1,
                StudentIdentifier = userEmail,
                StartedAt = DateTime.UtcNow,
                IsCompleted = false,
                IsAutoGraded = quiz.IsAutoGraded,
                IsPublished = false,
                IpAddress = ipAddress ?? "0.0.0.0",
                QuizId = quiz.Id,
                StudentId = student.Id,
                MaxScore = quiz.Questions.Sum(q => q.Points)
            };
            await _uow.QuizAttempts.AddAsync(attempt);
            await _uow.SaveAsync();

            return attempt.Id;
        }

        public async Task<QuizTakingViewModel> GetTakeViewModelAsync(int attemptId, int? questionId, int index)
        {
            var attempt = await _uow.QuizAttempts.GetAttemptByIdAsync(
                attemptId,
                includes: "Quiz,Quiz.Questions,Quiz.Questions.Choices,Answers");
            if (attempt == null || attempt.IsCompleted)
                throw new KeyNotFoundException("Attempt not found or already completed");

            var quiz = attempt.Quiz!;

            var nowUtc = DateTime.UtcNow;
            if (quiz.StartAt.HasValue && nowUtc < quiz.StartAt.Value)
                throw new InvalidOperationException("Quiz has not started yet.");
            if (quiz.EndAt.HasValue && nowUtc > quiz.EndAt.Value)
                throw new InvalidOperationException("Quiz has expired.");

            var endsAt = attempt.StartedAt.AddMinutes(quiz.DurationMintes);
            var orderedQuestions = quiz.Questions.OrderBy(q => q.OrderIndex).ToList();

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

            return vm;
        }

        public async Task<int> SubmitAsync(int attemptId, string? answersJson)
        {
            var attempt = await _uow.QuizAttempts.GetAttemptByIdAsync(
                attemptId,
                includes: "Quiz,Quiz.Questions,Quiz.Questions.Choices,Answers,Answers.Question,Answers.Choice");
            if (attempt == null)
                throw new KeyNotFoundException("Attempt not found");

            if (attempt.IsCompleted)
                return attempt.Id;

            if (!string.IsNullOrWhiteSpace(answersJson))
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<List<ClientAnswerDto>>(answersJson) ?? new();
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
                    attempt = await _uow.QuizAttempts.GetAttemptByIdAsync(
                        attemptId,
                        includes: "Quiz,Quiz.Questions,Quiz.Questions.Choices,Answers,Answers.Question,Answers.Choice");
                }
                catch
                {
                    // ignore bad payload and continue
                }
            }

            attempt.FinishedAt = DateTime.UtcNow;
            attempt.IsCompleted = true;

            decimal score = 0m;
            foreach (var q in attempt.Quiz!.Questions)
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

            return attempt.Id;
        }

        public async Task ExitAsync(int attemptId, string? answersJson)
        {
            var attempt = await _uow.QuizAttempts.GetAttemptByIdAsync(
                attemptId,
                includes: "Quiz,Quiz.Questions,Quiz.Questions.Choices,Answers,Answers.Question,Answers.Choice");
            if (attempt == null)
                throw new KeyNotFoundException("Attempt not found");

            if (attempt.IsCompleted)
                return;

            if (!string.IsNullOrWhiteSpace(answersJson))
            {
                try
                {
                    var payload = JsonSerializer.Deserialize<List<ClientAnswerDto>>(answersJson) ?? new();
                    foreach (var existing in attempt.Answers.ToList())
                    {
                        await _uow.Answers.DeleteAsync(existing.Id);
                    }
                    foreach (var p in payload)
                    {
                        if (p == null) continue;
                        if (p.choiceId.HasValue)
                        {
                            await _uow.Answers.AddAsync(new Answer { QuizAttemptId = attempt.Id, QuestionId = p.questionId, ChoiceId = p.choiceId, IsCorrect = null, IsGraded = false });
                        }
                        else if (!string.IsNullOrWhiteSpace(p.textAnswer))
                        {
                            await _uow.Answers.AddAsync(new Answer { QuizAttemptId = attempt.Id, QuestionId = p.questionId, TextAnswer = p.textAnswer, IsCorrect = null, IsGraded = false });
                        }
                    }
                }
                catch
                {
                    // ignore bad payload and continue
                }
            }

            // Keep attempt open and update timestamp
            attempt.IsCompleted = false;
            attempt.FinishedAt = null;
            attempt.UpdatedAt = DateTime.UtcNow;
            _uow.QuizAttempts.Update(attempt);
            await _uow.SaveAsync();
        }

        public async Task<QuizResultViewModel> GetResultAsync(int attemptId)
        {
            var attempt = await _uow.QuizAttempts.GetAttemptByIdAsync(
                attemptId,
                includes: "Quiz,Quiz.Questions,Quiz.Questions.Choices,Answers,Answers.Question,Answers.Choice");
            if (attempt == null)
                throw new KeyNotFoundException("Attempt not found");

            var autoGradedQuestions = attempt.Quiz.Questions
                .Where(q => q.QuestionType == QuestionType.MCQ || q.QuestionType == QuestionType.TrueFalse)
                .ToList();
            var autoMax = autoGradedQuestions.Sum(q => q.Points);
            var autoScore = attempt.Score ?? 0m; // attempt.Score is computed only from auto-graded questions
            var autoPct = autoMax > 0 ? Math.Round((autoScore / autoMax) * 100m, 2) : (decimal?)0;

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
                ShowScoreImmediately = attempt.Quiz.ShowScoreImmediatlely,
                Passed = attempt.Quiz.PassingScore.HasValue ? (attempt.Score ?? 0) >= attempt.Quiz.PassingScore.Value : false,
                AwaitingManualGrading = attempt.Answers.Any(a => a.Question.QuestionType == QuestionType.Essay || a.Question.QuestionType == QuestionType.ShortAnswer),
                AutoGradedMaxScore = autoMax,
                AutoGradedScore = autoScore,
                AutoGradedPercentage = autoPct
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

            return vm;
        }

        public async Task<StudentAccessViewModel> GetAccessLinkAsync(string token, string userId)
        {
            var quiz = await _uow.Quizzes.GetByAccessTokenAsync(token,
                includes: "Instructor.User,QuizAttempts");
            if (quiz == null)
                throw new KeyNotFoundException("Invalid access token");

            var student = await _uow.Students.GetByUserIdAsync(userId);
            var completedAttemptsCount = student == null
                ? 0
                : await _uow.QuizAttempts.CountCompletedAttemptsForStudentAsync(quiz.Id, student.Id);

            var now = DateTime.UtcNow;
            string? validation = null;
            if (!quiz.IsPublished) validation = "Quiz is not published yet.";
            else if (quiz.StartAt.HasValue && now < quiz.StartAt.Value.ToUniversalTime()) validation = $"Quiz will open at {quiz.StartAt.Value:u}.";
            else if (quiz.EndAt.HasValue && now > quiz.EndAt.Value.ToUniversalTime()) validation = "Quiz is no longer available.";
            else if (!quiz.AllowMultipleAttempts && completedAttemptsCount > 0) validation = "You have already completed this quiz.";
            else if (quiz.AllowMultipleAttempts && quiz.MaxAttempts.HasValue && completedAttemptsCount >= quiz.MaxAttempts.Value) validation = "You have reached the maximum number of attempts.";

            var vm = new StudentAccessViewModel
            {
                AccessToken = token,
                Title = quiz.Title,
                Description = quiz.Description,
                InstructorName = $"{quiz.Instructor?.User?.FirstName} {quiz.Instructor?.User?.LastName}".Trim(),
                DurationMinutes = quiz.DurationMintes,
                StartAt = quiz.StartAt,
                EndAt = quiz.EndAt,
                IsPublished = quiz.IsPublished,
                AllowMultipleAttempts = quiz.AllowMultipleAttempts,
                MaxAttempts = quiz.MaxAttempts,
                AlreadyAttempted = completedAttemptsCount > 0,
                ValidationMessage = validation
            };

            return vm;
        }

        public async Task<List<QuizAttempt>> GetRecentAttemptsForStudentAsync(int studentId, int take)
        {
            return await _uow.QuizAttempts.GetRecentAttemptsForStudentAsync(studentId, take, includes: "Quiz");
        }

        public async Task<List<QuizAttempt>> GetRecentAttemptsForUserAsync(string userId, int take)
        {
            var student = await _uow.Students.GetByUserIdAsync(userId);
            if (student == null) return new List<QuizAttempt>();
            return await _uow.QuizAttempts.GetRecentAttemptsForStudentAsync(student.Id, take, includes: "Quiz");
        }

        public async Task<StudentResultsOverviewViewModel> GetResultsOverviewAsync(string userId, int recentTake = 10)
        {
            var student = await _uow.Students.GetByUserIdAsync(userId);
            if (student == null)
            {
                return new StudentResultsOverviewViewModel();
            }

            var query = _uow.QuizAttempts
                .GetQueryable(includes: "Quiz,Answers")
                .Where(a => a.StudentId == student.Id);

            var completed = await query.Where(a => a.IsCompleted && a.FinishedAt != null).ToListAsync();

            decimal average = 0m;
            decimal? best = null;
            TimeSpan totalTime = TimeSpan.Zero;

            if (completed.Count > 0)
            {
                var percentages = completed.Where(a => a.Percentage.HasValue).Select(a => a.Percentage!.Value).ToList();
                if (percentages.Count > 0)
                {
                    average = Math.Round(percentages.Average(), 2);
                    best = percentages.Max();
                }

                foreach (var a in completed)
                {
                    if (!a.FinishedAt.HasValue) continue;
                    var end = a.FinishedAt.Value;
                    var duration = end - a.StartedAt;
                    if (duration > TimeSpan.Zero)
                        totalTime += duration;
                }
            }

            var recent = await query
                .OrderByDescending(a => a.FinishedAt ?? a.StartedAt)
                .Take(recentTake)
                .Select(a => new StudentResultsOverviewViewModel.RecentAttemptItem
                {
                    AttemptId = a.Id,
                    QuizId = a.QuizId,
                    QuizTitle = a.Quiz.Title,
                    Percentage = a.Quiz.ShowScoreImmediatlely
                        ? (
                            (
                                (a.Quiz.Questions.Where(q => q.QuestionType == QuestionType.MCQ || q.QuestionType == QuestionType.TrueFalse).Sum(q => (decimal?)q.Points) ?? 0) > 0
                            )
                                ? Math.Round(
                                    (((a.Score ?? 0) / (a.Quiz.Questions.Where(q => q.QuestionType == QuestionType.MCQ || q.QuestionType == QuestionType.TrueFalse).Sum(q => (decimal?)q.Points) ?? 0)) * 100m)
                                  , 2)
                                : (decimal?)0
                          )
                        : null,
                    QuestionsCount = a.Quiz.Questions.Count,
                    Duration = (a.FinishedAt ?? a.UpdatedAt) - a.StartedAt,
                    FinishedAt = a.FinishedAt,
                    IsCompleted = a.IsCompleted,
                    Status = a.IsCompleted
                        ? (a.Answers.Any(ans => (ans.Question.QuestionType == QuestionType.Essay || ans.Question.QuestionType == QuestionType.ShortAnswer) && !ans.IsGraded)
                            ? "Pending Grading" : "Completed")
                        : "Exited",
                    DisplayAt = (a.FinishedAt ?? a.UpdatedAt),
                    ShowScoreImmediately = a.Quiz.ShowScoreImmediatlely
                })
                .ToListAsync();

            return new StudentResultsOverviewViewModel
            {
                AverageScorePercentage = average,
                CompletedQuizzesCount = completed.Count,
                BestScorePercentage = best,
                TotalTimeSpent = totalTime,
                RecentAttempts = recent
            };
        }

        private class ClientAnswerDto
        {
            public int questionId { get; set; }
            public int? choiceId { get; set; }
            public string? textAnswer { get; set; }
        }
    }
}


