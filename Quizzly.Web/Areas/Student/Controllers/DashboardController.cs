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
    public class DashboardController : Controller
    {
        private readonly IStudentQuizService _studentQuizService;
        private readonly UserManager<ApplicationUser> _userManager;
        public DashboardController(IStudentQuizService studentQuizService, UserManager<ApplicationUser> userManager)
        {
            _studentQuizService = studentQuizService; _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            ViewBag.FullName = $"{user?.FirstName} {user?.LastName}";
            var recent = await _studentQuizService.GetRecentAttemptsForUserAsync(user!.Id, 5);
            return View(recent);
        }
    }
}


