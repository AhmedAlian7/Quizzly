using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.Services.Implementions;
using Quizzly.Business.ViewModels;
using Quizzly.Business.ViewModels.Instructor;
using Quizzly.DataAccess.Constants;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
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
                return RedirectToAction("Index");

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
                    MaxPoints = a.Question?.Points ?? a.MaxPoints,
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
                return View("ManualGrading", model);

            // Grade all answers in one submission
            foreach (var answer in model.Answers)
            {
                if (answer.PointsAwarded.HasValue)
                {
                    await _manualGradingService.ManualGradeAnswerAsync(answer.AnswerId, answer.PointsAwarded.Value);
                }
            }

            await _manualGradingService.UpdateAttemptTotalScoreAsync(model.AttemptId);

/*            var refreshedAttempts = await _manualGradingService.GetPendingAttemptsAsync();
*/

            TempData["SuccessMessage"] = "Manual grading saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
