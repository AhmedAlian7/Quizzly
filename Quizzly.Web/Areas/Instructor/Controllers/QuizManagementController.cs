using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        private readonly IQuizCategoriesService _quizCategoriesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizManagementController( IInstructorManagementService instructorManagementService , UserManager<ApplicationUser> userManager , IQuizCategoriesService quizCategoriesService)
        {
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
            _quizCategoriesService = quizCategoriesService;
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
        public async Task<IActionResult> Create()
        {
            var categories = await _quizCategoriesService.GetAllAsync();

            if (!categories.Any())
            {
                TempData["NoCategories"] = "You must create at least one quiz category before creating a quiz.";
                return RedirectToAction("Create", "QuizCategoryManagement", new { area = "Instructor" });
            }

            var model = new AddQuizDto
            {
                Categories = categories.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                })
            };

            return View(model);

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
