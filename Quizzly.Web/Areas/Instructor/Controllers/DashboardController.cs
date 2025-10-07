using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Implementions;
using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Entities;
using System.Threading.Tasks;

namespace Quizzly.Web.Areas.Instructor.Controllers
{

    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class DashboardController : Controller
    {
        private readonly IInstructorManagementService _instructorManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public DashboardController(IInstructorManagementService instructorManagementService, UserManager<ApplicationUser> userManager)
        {
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService.GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            var models = await _instructorManagementService
                .GetInstructorDashboardAsync(instructor.Id);
            return View(models);
        }
    }
}
