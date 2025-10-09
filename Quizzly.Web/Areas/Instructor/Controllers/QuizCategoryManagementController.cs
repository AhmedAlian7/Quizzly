using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.Business.ViewModels.QuizCategories;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = AppRoles.Instructor)]
    public class QuizCategoryManagementController : Controller
    {
        private readonly IInstructorManagementService _instructorManagementService;
        private readonly IQuizCategoriesService _QuizCategoriesService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizCategoryManagementController(IInstructorManagementService instructorManagementService , UserManager<ApplicationUser> userManager, IQuizCategoriesService quizCategoriesService)
        {
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
            _QuizCategoriesService = quizCategoriesService;
        }
       
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            var categories = await _QuizCategoriesService
                .GetAllByInstructorIdAsync(instructor.Id);

            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new AddQuizCategoryDto());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AddQuizCategoryDto addQuizCategoryDto)
        {
            if (!ModelState.IsValid)
                return View(addQuizCategoryDto);

            var user = await _userManager.GetUserAsync(User);

            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            await _instructorManagementService.AddQuizCategoryAsync(instructor.Id, addQuizCategoryDto);
            return RedirectToAction("Index");
        }
    }
}
