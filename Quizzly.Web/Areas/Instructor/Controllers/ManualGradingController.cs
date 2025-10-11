using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Instructor;
using Quizzly.DataAccess.Constants;

[Area("Instructor")]
[Authorize(Roles = AppRoles.Instructor)]
public class ManualGradingController : Controller
{
    private readonly IManualGradingService _manualGradingService;

    public ManualGradingController(IManualGradingService manualGradingService)
    {
        _manualGradingService = manualGradingService;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var attempts = await _manualGradingService.GetPendingAttemptsAsync();
        var viewModel = attempts.Select(a => new ManualGradingListDto
        {
            AttemptId = a.Id,
            QuizTitle = a.Quiz.Title,
            StudentName = a.Student.User.FirstName + " " + a.Student.User.LastName,
            IsGraded = a.Answers.All(ans => ans.PointsAwarded.HasValue)
        }).ToList();

        return View(viewModel);
    }

    // GET: ManualGrading/Attempt/5
    [HttpGet]
    public async Task<IActionResult> Attempt(int attemptId)
    {
        var answers = await _manualGradingService.GetAnswersNeedingManualGradingAsync(attemptId);

        if (answers == null || !answers.Any())
        {
            TempData["InfoMessage"] = "No answers need manual grading for this attempt.";
            return RedirectToAction("Index");
        }

        var first = answers.First();
        var viewModel = new ManualGradingDto
        {
            AttemptId = attemptId,
            StudentName = first.QuizAttempt.Student.User.FirstName + " " + first.QuizAttempt.Student.User.LastName,
            QuizTitle = first.QuizAttempt.Quiz.Title,
            Answers = answers.Select(a => new ManualAnswerDto
            {
                AnswerId = a.Id,
                QuestionText = a.Question.Text,
                StudentAnswer = a.TextAnswer,
                MaxPoints = a.Question?.Points ?? 0, // Always get from Question
                PointsAwarded = a.PointsAwarded
            }).ToList()
        };

        return View("ManualGrading", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Submit(ManualGradingDto model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Invalid form data. Please check your inputs.";
            return View("ManualGrading", model);
        }

        try
        {
            // Grade all answers in one submission
            foreach (var answer in model.Answers)
            {
                if (answer.PointsAwarded.HasValue)
                {
                    await _manualGradingService.ManualGradeAnswerAsync(answer.AnswerId, answer.PointsAwarded.Value);
                }
            }

            await _manualGradingService.UpdateAttemptTotalScoreAsync(model.AttemptId);

            TempData["SuccessMessage"] = "Manual grading saved successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Error saving grades: {ex.Message}";

            // Reload the data to display the form again
            var answers = await _manualGradingService.GetAnswersNeedingManualGradingAsync(model.AttemptId);
            if (answers != null && answers.Any())
            {
                var first = answers.First();
                model.StudentName = first.QuizAttempt.Student.User.FirstName + " " + first.QuizAttempt.Student.User.LastName;
                model.QuizTitle = first.QuizAttempt.Quiz.Title;

                // Update MaxPoints from actual Question data
                foreach (var answer in model.Answers)
                {
                    var actualAnswer = answers.FirstOrDefault(a => a.Id == answer.AnswerId);
                    if (actualAnswer != null)
                    {
                        answer.MaxPoints = actualAnswer.Question?.Points ?? 0;
                    }
                }
            }

            return View("ManualGrading", model);
        }
    }
}