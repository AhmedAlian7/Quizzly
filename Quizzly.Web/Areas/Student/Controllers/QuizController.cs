using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = AppRoles.Student)]
    public class QuizController : Controller
    {
        private readonly IStudentQuizService _studentQuizService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizController(IStudentQuizService studentQuizService, UserManager<ApplicationUser> userManager)
        {
            _studentQuizService = studentQuizService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Start(string token)
        {
            var user = await _userManager.GetUserAsync(User);
            try
            {
                var IpAddress =GetClientIp();

                var attemptId = await _studentQuizService.StartAttemptAsync(token, user!.Id, user.Email!, IpAddress);
                return RedirectToAction(nameof(Take), new { attemptId, index = 0 });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Link", "Access", new { area = "Student", token });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Take(int attemptId, int? questionId = null, int index = 0)
        {
            try
            {
                var vm = await _studentQuizService.GetTakeViewModelAsync(attemptId, questionId, index);
                return View(vm);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(int attemptId, string? answersJson)
        {
            var attemptIdResult = await _studentQuizService.SubmitAsync(attemptId, answersJson);
            return RedirectToAction(nameof(Result), new { attemptId = attemptIdResult });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int attemptId)
        {
            var vm = await _studentQuizService.GetResultAsync(attemptId);
            return View(vm);
        }

        private string GetClientIp()
        {
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            // Check for X-Forwarded-For (in case of reverse proxy)
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
            {
                var forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    ip = forwardedFor.Split(',')[0]; // take first IP
                }
            }

            return string.IsNullOrEmpty(ip) ? "0.0.0.0" : ip;
        }


    }
}