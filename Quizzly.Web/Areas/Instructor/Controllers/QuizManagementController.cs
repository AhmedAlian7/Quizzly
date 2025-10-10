using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;
using System;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = AppRoles.Instructor)]
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

            var models = await _quizCategoriesService
                .GetAllByInstructorIdAsync(instructor.Id);

            return View(models);
        }

        public async Task<IActionResult> QuizzesByCategory(int CategoryId)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            if (instructor == null)
                return NotFound("Instructor profile not found.");

            var models = await _quizService
                .GetQuizzesByCategory(CategoryId, instructor.Id);

            return View(models);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService
                   .GetInstructorByUserIdAsync(user.Id);


            var categories = await _quizCategoriesService
                .GetAllByInstructorIdAsync(instructor.Id);

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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddQuizDto addQuizDto , string formAction)
        {
             var user = await _userManager.GetUserAsync(User);
             var instructor = await _instructorManagementService
                    .GetInstructorByUserIdAsync(user.Id);

             if (!ModelState.IsValid)
             {
                 var errors = ModelState.Values
                     .SelectMany(v => v.Errors)
                     .Select(e => e.ErrorMessage)
                     .ToList();
             
                 // Reload categories
                 addQuizDto.Categories = (await _quizCategoriesService.GetAllByInstructorIdAsync(instructor.Id))
                     .Select(c => new SelectListItem
                     {
                         Value = c.Id.ToString(),
                         Text = c.Name
                     });

                 return Json(new { success = false, errors = errors });
             }

            

             if (instructor == null)
                 return Json(new { success = false, error = "Instructor profile not found." });

             var quizId = await _instructorManagementService
                 .AddQuizAsync(instructor.Id, addQuizDto);

             if (formAction == "publish")
             {
                var token = await _quizService.PublishQuizAsync(quizId);
                return View("Token", token);
             }

             return RedirectToAction("Index", "QuizCategoryManagement", new { area = "Instructor" });

        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
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

        [HttpGet]
        public async Task<IActionResult> Update(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService
                .GetInstructorByUserIdAsync(user.Id);

            var quizDto = await _quizService
                .GetQuizByIdAsync(id);

            if (quizDto == null)
                return NotFound("Quiz not found.");

            var categories = await _quizCategoriesService.GetAllByInstructorIdAsync(instructor.Id);

            quizDto.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name,
                Selected = (c.Id == quizDto.CategoryId)
            });

            return View(quizDto);
        }

        [HttpPost]
        public async Task<IActionResult> Update(QuizDetailsDto quizDetailsDto)
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService
                 .GetInstructorByUserIdAsync(user.Id);

            quizDetailsDto.Categories = (await _quizCategoriesService.GetAllByInstructorIdAsync(instructor.Id))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Name
                });

            if (!ModelState.IsValid)
            {
                return View(quizDetailsDto);
            }

            var existingQuiz = await _quizService
                .GetQuizByIdAsync(quizDetailsDto.Id);

            if (existingQuiz == null)
                return NotFound("Quiz not found.");

            await _quizService.UpdateQuizAsync(quizDetailsDto);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int quizId)
        {

            await _quizService.DeleteQuizAsync(quizId);
            TempData["SuccessMessage"] = "Quiz deleted successfully.";
            return RedirectToAction("Index");
        }


    }
}
