using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.ViewModels.Student;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class AccessController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccessController(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Link(string token, string returnUrl = null)
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                var loginUrl = Url.Action("Login", "Account", new { area = "Authentication", returnUrl = Url.Action(nameof(Link), new { token }) });
                return Redirect(loginUrl!);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null || !(await _userManager.IsInRoleAsync(user, AppRoles.Student)))
            {
                TempData["ErrorMessage"] = "Not authorized. Please log in with a student account.";
                return View("~/Views/Shared/NotAuthorized.cshtml");
            }

            var quiz = await _uow.Quizzes.GetByAccessTokenAsync(token, includeRelations: true);
            if (quiz == null)
            {
                return View("~/Areas/Student/Views/Access/InvalidLink.cshtml");
            }

            var student = (await _uow.Students.GetAllAsync("")).FirstOrDefault(s => s.UserId == user.Id);
            var attempts = quiz.QuizAttempts.Where(a => a.StudentId == (student?.Id ?? 0)).ToList();

            var now = DateTime.UtcNow;
            string? validation = null;
            if (!quiz.IsPublished) validation = "Quiz is not published yet.";
            else if (quiz.StartAt.HasValue && now < quiz.StartAt.Value.ToUniversalTime()) validation = $"Quiz will open at {quiz.StartAt.Value:u}.";
            else if (quiz.EndAt.HasValue && now > quiz.EndAt.Value.ToUniversalTime()) validation = "Quiz is no longer available.";
            else if (!quiz.AllowMultipleAttempts && attempts.Any(a => a.IsCompleted)) validation = "You have already completed this quiz.";
            else if (quiz.AllowMultipleAttempts && quiz.MaxAttempts.HasValue && attempts.Count(a => a.IsCompleted) >= quiz.MaxAttempts.Value) validation = "You have reached the maximum number of attempts.";

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
                AlreadyAttempted = attempts.Any(a => a.IsCompleted),
                ValidationMessage = validation
            };

            return View("~/Areas/Student/Views/Access/Link.cshtml", vm);
        }
    }
}


