using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using System.Threading.Tasks;

namespace Quizzly.Web.Controllers.Areas.Instructor.Controllers
{
    
    [Area("Instructor")]
    public class DashboardController : Controller
    {
        private readonly IInstructorManagementService _instructorDashboardService;

        public DashboardController(IInstructorManagementService instructorDashboardService)
        {
            _instructorDashboardService = instructorDashboardService;
        }
        public async Task<IActionResult> Index(int InstructorId)
        {
            var models = await _instructorDashboardService
                .GetInstructorDashboardAsync(InstructorId);
            return View(models);
        }
    }
}
