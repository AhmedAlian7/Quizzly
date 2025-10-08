using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Web.Areas.Student.Controllers
{
    [Area("Student")]
    [Authorize(Roles = AppRoles.Student)]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _uow;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(IUnitOfWork uow, UserManager<ApplicationUser> userManager)
        {
            _uow = uow; _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = (await _uow.Students.GetAllAsync("")).FirstOrDefault(s => s.UserId == user!.Id);
            var attempts = await _uow.QuizAttempts.GetAllAsync(includes: "Quiz");
            var myAttempts = attempts.Where(a => a.StudentId == (student?.Id ?? 0)).OrderByDescending(a => a.CreatedAt).Take(5).ToList();
            return View(myAttempts);
        }
    }
}


