using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class QuizManagementController : Controller
    {
        private readonly IInstructorManagementService _instructorManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizManagementController( IInstructorManagementService instructorManagementService , UserManager<ApplicationUser> userManager)
        {
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            var models = await _instructorManagementService.GetAllQuizzesAsync(instructor.Id);
            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AddQuizDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddQuizDto addQuizDto)
        {
            if (!ModelState.IsValid)
                return View(addQuizDto);

            var user = await _userManager.GetUserAsync(User);

            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            await _instructorManagementService.AddQuizAsync(instructor.Id, addQuizDto);
            return RedirectToAction("Index");
        }
    }
}
