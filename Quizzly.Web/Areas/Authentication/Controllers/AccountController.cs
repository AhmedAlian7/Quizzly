using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Quizzly.Business.ViewModels.Authentication;
using Quizzly.DataAccess.Constants;
using Quizzly.DataAccess.Entities;

using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Quizzly.Web.Areas.Authentication.Controllers
{
    [Area("Authentication")]
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly DataAccess.Repositories.Interfaces.IUnitOfWork _unitOfWork;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DataAccess.Repositories.Interfaces.IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public IActionResult Register()
        {
            var roleList = AppRoles.All.Select(r => new SelectListItem
            {
                Text = r,
                Value = r
            }).ToList();
            var registerViewModel = new RegisterViewModel { RolesList = roleList };
            return View(registerViewModel); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            // Additional server-side validations
            if (!register.AcceptTerms)
            {
                ModelState.AddModelError(nameof(register.AcceptTerms), "You must accept the Terms and Conditions.");
            }

            if (string.IsNullOrWhiteSpace(register.Role) || !AppRoles.All.Contains(register.Role))
            {
                ModelState.AddModelError(nameof(register.Role), "Please select a valid role.");
            }

            if (!ModelState.IsValid)
            {
                register.RolesList = AppRoles.All
                    .Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r
                    }).ToList();
                TempData["ErrorMessage"] = "Please make sure all fields are valid.";
                return View(register);
            }

            var user = new ApplicationUser
            {
                Email = register.Email,
                UserName = await GenerateUniqueUserNameAsync(register.Email.Split('@')[0]),
                FirstName = register.FirstName,
                LastName = register.LastName,
                CreatedAt = DateTime.Now,
            };
            var result = await _userManager.CreateAsync(user, register.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, register.Role);

                // Create domain entity based on selected role
                if (register.Role == AppRoles.Instructor)
                {
                    var instructor = new DataAccess.Entities.Instructor
                    {
                        UserId = user.Id,
                        Title = register.InstructorTitle
                    };
                    await _unitOfWork.Instructors.AddAsync(instructor);
                    await _unitOfWork.SaveAsync();
                    return RedirectToAction("Index", "Dashboard", new { area = "Instructor", InstructorId = instructor.Id });
                }
                else if (register.Role == AppRoles.Student)
                {
                    var student = new DataAccess.Entities.Student
                    {
                        UserId = user.Id,
                        StudentNumber = register.StudentNumber
                    };
                    await _unitOfWork.Students.AddAsync(student);
                    await _unitOfWork.SaveAsync();
                }
                await _signInManager.SignInAsync(user, isPersistent: true);
                TempData["SuccessMessage"] = "Account Created successfully!";
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError("", error.Description);
                register.RolesList = AppRoles.All
                    .Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r
                    }).ToList();
                TempData["ErrorMessage"] = "Failed to create account.";
                return View(nameof(Register), register);
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
                return View(model);
            }

            // lockout check
            if (_userManager.SupportsUserLockout &&
                user.LockoutEnd.HasValue &&
                user.LockoutEnd.Value > DateTimeOffset.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "This account is locked. Try again later.");
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return LocalRedirect(returnUrl);

                if (await _userManager.IsInRoleAsync(user, AppRoles.Admin))
                    return RedirectToAction("Index", "Product", new { area = "Admin" });

                if (await _userManager.IsInRoleAsync(user, AppRoles.Instructor))
                {
                    TempData["SuccessMessage"] = "Login Successfully, Wellcome Back!";
                    var instructor = (await _unitOfWork.Instructors.GetAllAsync("")).FirstOrDefault(i => i.UserId == user.Id);
                    if (instructor != null)
                        return RedirectToAction("Index", "Dashboard", new { area = "Instructor", InstructorId = instructor.Id });
                }
                if (await _userManager.IsInRoleAsync(user, AppRoles.Student))
                {
                    TempData["SuccessMessage"] = "Login Successfully, Wellcome Back!";
                    var student = (await _unitOfWork.Students.GetAllAsync("")).FirstOrDefault(s => s.UserId == user.Id);
                    if (student != null)
                        return RedirectToAction("Index", "Dashboard", new { area = "Student" });
                }

            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Too many attempts. Your account is temporarily locked.");
                return View(model);
            }

            ModelState.AddModelError(string.Empty, "Email or password is incorrect.");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {

            await _signInManager.SignOutAsync();
            TempData["SuccessMessage"] = "Logout Successfully";
            return RedirectToAction("Login", "Account", new { area = "Authentication" });
        }

        public IActionResult ExternalLogin(string provider, string? returnUrl = "")
        {
            var redirectUrl = Url.Action(nameof(ExternalLoginCallback), "Account", new { returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl ??= Url.Content("~/");
            if (remoteError != null)
            {
                TempData["error"] = $"Login failed: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["error"] = "Error while Creating account.";
                return RedirectToAction(nameof(Login));
            }

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (signInResult.Succeeded)
            {
                TempData["success1"] = $"Login with {info.LoginProvider} successful";

                var userSignedIn = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return RedirectToDashboardOrHome(userSignedIn!, returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@{info.LoginProvider}.com";
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];
            var userName = await GenerateUniqueUserNameAsync(name);
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    CreatedAt = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    TempData["error"] = "Error while creating user.";
                    return RedirectToAction(nameof(Login));
                }
                await _userManager.AddToRoleAsync(user, AppRoles.Instructor);
            }

            var alreadyLinked = (await _userManager.GetLoginsAsync(user))
                .Any(login => login.LoginProvider == info.LoginProvider && login.ProviderKey == info.ProviderKey);

            if (!alreadyLinked)
            {
                var linkResult = await _userManager.AddLoginAsync(user, info);
                if (!linkResult.Succeeded)
                {
                    TempData["error"] = "Error while linking external login.";
                    return RedirectToAction(nameof(Login));
                }
            }

            await _signInManager.SignInAsync(user, isPersistent: true);
            TempData["success1"] = $"Login with {info.LoginProvider} successful";
            return RedirectToDashboardOrHome(user, returnUrl);
        }
        private IActionResult RedirectToDashboardOrHome(ApplicationUser user, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                return LocalRedirect(returnUrl);

            if (_userManager.IsInRoleAsync(user, AppRoles.Admin).Result)
                return RedirectToAction("Dashboard", "User", new { area = "Admin" });

            return RedirectToAction("Index", "Home", new { area = "Customer" });
        }

        private async Task<string> GenerateUniqueUserNameAsync(string rawName)
        {
            var cleaned = Regex.Replace(rawName, @"[^a-zA-Z0-9]", "");

            if (string.IsNullOrWhiteSpace(cleaned))
                cleaned = "user" + Guid.NewGuid().ToString("N").Substring(0, 6);

            string finalName = cleaned;
            int i = 1;

            while (await _userManager.FindByNameAsync(finalName) != null)
            {
                finalName = $"{cleaned}{i}";
                i++;
            }
            return finalName;
        }

        // Remote Validation for RegisterViewModel
        public async Task<IActionResult> IsEmailInUse(string Email)
        {

            var user = await _userManager.FindByEmailAsync(Email);
            if (user != null)
            {
                return Json($"Email '{Email}' is already in use.");
            }
            return Json(true);


        }

        [HttpGet]
        public IActionResult AccessDenied(string returnUrl = "")
        {
            return View();
        }
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        public IActionResult DataDeletion()
        {
            return View();
        }

    }
}
