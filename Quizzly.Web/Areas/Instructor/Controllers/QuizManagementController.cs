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
        private readonly IQuizService _quizService;
        private readonly UserManager<ApplicationUser> _userManager;

        public QuizManagementController( IInstructorManagementService instructorManagementService , UserManager<ApplicationUser> userManager , IQuizCategoriesService quizCategoriesService , IQuizService quizService)
        {
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
            _quizCategoriesService = quizCategoriesService;
            _quizService = quizService;
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

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var quizDto = await _quizService
                .GetQuizByIdAsync(id);

            if (quizDto == null)
                return NotFound("Quiz not found.");

            return View(quizDto);
        }

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var quizDto = await _quizService
                .GetQuizByIdAsync(id);

            if (quizDto == null)
                return NotFound("Quiz not found.");

            quizDto.Categories = (await _quizCategoriesService.GetAllAsync())
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

            return View(quizDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(QuizDetailsDto quizDetailsDto)
        {
            if (!ModelState.IsValid)
                return View(quizDetailsDto);

            var existingQuiz = await _quizService
                .GetQuizByIdAsync(quizDetailsDto.Id);

            if (existingQuiz == null)
                return NotFound("Quiz not found.");

          

            await _quizService.UpdateQuizAsync(quizDetailsDto);
            return RedirectToAction("Index");
        }


    }
}
