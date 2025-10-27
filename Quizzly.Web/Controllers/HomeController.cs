using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;
using Quizzly.Web.Models;

namespace Quizzly.Web.Controllers
{
    public class HomeController : Controller
    {
        public HomeController()
        {
        }

        public IActionResult Index()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return RedirectToAction("Login", "Account", new { area = "Authentication" });

            return RedirectToRoleHome();
        }

        private IActionResult RedirectToRoleHome()
        {
            if (User.IsInRole(AppRoles.Instructor))
                return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });

            if (User.IsInRole(AppRoles.Student))
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });

            // fallback
            return RedirectToAction("Login", "Account", new { area = "Authentication" });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
