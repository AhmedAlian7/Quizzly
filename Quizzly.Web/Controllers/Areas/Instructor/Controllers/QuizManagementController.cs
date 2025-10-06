using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Quiz;

namespace Quizzly.Web.Controllers.Areas.Instructor.Controllers
{
    public class QuizManagementController : Controller
    {
        private readonly IInstructorManagementService _instructorManagementService;

        public QuizManagementController(IInstructorManagementService instructorManagementService)
        {
            _instructorManagementService = instructorManagementService;
        }

        public async Task<IActionResult> Index(int InstructorId)
        {
            var models = await _instructorManagementService
                .GetAllQuizzesAsync(InstructorId);

            return View(models);
        }

        [HttpGet]
        public IActionResult Create()
        {
           return View(new AddQuizDto());
        }


        [HttpPost]
        public async Task<IActionResult> Create(int InstructorId , AddQuizDto addQuizDto)
        {
           
            if (ModelState.IsValid)
            {
                await _instructorManagementService
                    .AddQuizAsync(InstructorId, addQuizDto);
                return RedirectToAction("Index");
            }
            return View();
        }
    }
}
