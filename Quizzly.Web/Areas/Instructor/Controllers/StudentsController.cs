using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Implementions;
using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Web.Areas.Instructor.Controllers
{
    [Area("Instructor")]
    [Authorize(Roles = AppRoles.Instructor)]
    public class StudentsController : Controller
    {
        private readonly IStudentInstructorService _studentInstructorService;
        private readonly IInstructorManagementService _instructorManagementService;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentsController(IStudentInstructorService studentInstructorService, IInstructorManagementService instructorManagementService, UserManager<ApplicationUser> userManager)
        {
            _studentInstructorService = studentInstructorService;
            _instructorManagementService = instructorManagementService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var instructor = await _instructorManagementService
                 .GetInstructorByUserIdAsync(user.Id);

            var model = await _studentInstructorService
                .studentsTableDtos(instructor.Id);

            return View(model);
        }
    }
}
