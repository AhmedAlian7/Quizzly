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
    public class ResultsController : Controller
    {
        private readonly IStudentQuizService _studentQuizService;
        private readonly UserManager<ApplicationUser> _userManager;

        public ResultsController(IStudentQuizService studentQuizService, UserManager<ApplicationUser> userManager)
        {
            _studentQuizService = studentQuizService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int page = 1)
        {
            var user = await _userManager.GetUserAsync(User);
            var vm = await _studentQuizService.GetResultsOverviewAsync(user!.Id, page, Numbers.DefaultPageSize - 5);
            return View(vm);
        }
    }
}


