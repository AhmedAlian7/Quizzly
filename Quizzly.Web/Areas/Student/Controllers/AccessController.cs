using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Web.Areas.Student.Controllers
{
    [Area("Student")]
    public class AccessController : Controller
    {
        private readonly IStudentQuizService _studentQuizService;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccessController(IStudentQuizService studentQuizService, UserManager<ApplicationUser> userManager)
        {
            _studentQuizService = studentQuizService;
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

                return RedirectToAction("AccessDenied", "Account", new { area = "Authentication" });
            }
            try
            {
                var vm = await _studentQuizService.GetAccessLinkAsync(token, user.Id);
                return View(vm);
            }
            catch
            {
                return RedirectToAction(nameof(InvalidLink));
            }
        }

        public IActionResult InvalidLink()
        {
            return View();
        }
    }
}


