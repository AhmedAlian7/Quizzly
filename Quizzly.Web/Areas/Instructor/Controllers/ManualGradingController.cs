using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Implementions;
using Quizzly.Business.ViewModels;
using Quizzly.Business.ViewModels.Instructor;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
    public class ManualGradingController : Controller
    {
        private readonly ManualGradingService _manualGradingService;

        public ManualGradingController(ManualGradingService manualGradingService)
        {
            _manualGradingService = manualGradingService;
        }

        // GET: ManualGrading/Attempt/5
        [HttpGet]
        [Route("ManualGrading/Attempt/{attemptId}")]
        public async Task<IActionResult> Attempt(int attemptId)
        {
            // Get the attempt (with its answers and related questions)
            var attempt = await _manualGradingService.GetAttemptByIdAsync(attemptId, "Answers.Question,Student,Quiz");
            if (attempt == null)
                return NotFound();

            // Prepare data for the view
            var viewModel = new ManualGradingDto
            {   
                AttemptId = attempt.Id,
                StudentName = ( attempt.Student.User.FirstName + " " + attempt.Student.User.LastName) ,
                QuizTitle = attempt.Quiz.Title,
                Answers = attempt.Answers.Select(a => new ManualAnswerDto
                {
                    AnswerId = a.Id,
                    QuestionText = a.Question.Text,
                    StudentAnswer = a.TextAnswer,
                    MaxPoints = a.Question.Points,
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

            TempData["SuccessMessage"] = "Manual grading saved successfully!";
            return RedirectToAction("Attempt", new { attemptId = model.AttemptId });
        }
    }
}
