using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using System.Threading.Tasks;

namespace Quizzly.Web.Controllers.Areas.Instructor.Controllers
{
    
    [Area("Instructor")]
    public class DashboardController : Controller
    {
        private readonly IInstructorDashboardService _instructorDashboardService;

        public DashboardController(IInstructorDashboardService instructorDashboardService)
        {
            _instructorDashboardService = instructorDashboardService;
        }
        public async Task<IActionResult> Index(int InstructorId)
        {
            var models = await _instructorDashboardService.GetInstructorDashboard(InstructorId);
            return View(models);
        }
    }
}
