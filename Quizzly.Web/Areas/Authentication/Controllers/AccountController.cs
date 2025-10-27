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
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToRoleHome();

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
                return RedirectToDashboardOrHome(user, null);
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
            if (User.Identity?.IsAuthenticated == true)
                return RedirectToRoleHome();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }
        private IActionResult RedirectToRoleHome()
        {
            if (User.IsInRole(AppRoles.Instructor))
                return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });
            if (User.IsInRole(AppRoles.Student))
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });
            return RedirectToAction("Index", "Home");
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

                TempData["SuccessMessage"] = "Login Successfully, Wellcome Back!";
                return RedirectToDashboardOrHome(user, returnUrl);

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
                TempData["ErrorMessage"] = $"Login failed: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                TempData["ErrorMessage"] = "Error while retrieving external login information.";
                return RedirectToAction(nameof(Login));
            }

            // Check if user already exists and has completed registration
            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: true);
            if (signInResult.Succeeded)
            {
                TempData["SuccessMessage"] = $"Login with {info.LoginProvider} successful";
                var userSignedIn = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                return RedirectToDashboardOrHome(userSignedIn!, returnUrl);
            }

            // User doesn't exist or hasn't completed registration - redirect to completion form
            var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? $"{info.ProviderKey}@{info.LoginProvider}.com";
            var name = info.Principal.FindFirstValue(ClaimTypes.Name) ?? email.Split('@')[0];
            var nameParts = name.Split(' ');
            var firstName = nameParts.Length > 0 ? nameParts[0] : "";
            var lastName = nameParts.Length > 1 ? string.Join(" ", nameParts.Skip(1)) : "";

            var completionViewModel = new ExternalLoginCompletionViewModel
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                ExternalLoginProvider = info.LoginProvider,
                ExternalLoginProviderKey = info.ProviderKey,
                RolesList = AppRoles.All.Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList()
            };

            // Store external login info in TempData for the completion form
            TempData["ExternalLoginProvider"] = info.LoginProvider;
            TempData["ExternalLoginProviderKey"] = info.ProviderKey;
            TempData["ExternalEmail"] = email;

            return View("ExternalLoginCompletion", completionViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginCompletion(ExternalLoginCompletionViewModel model)
        {
            // Additional server-side validations
            if (!model.AcceptTerms)
            {
                ModelState.AddModelError(nameof(model.AcceptTerms), "You must accept the Terms and Conditions.");
            }

            if (string.IsNullOrWhiteSpace(model.Role) || !AppRoles.All.Contains(model.Role))
            {
                ModelState.AddModelError(nameof(model.Role), "Please select a valid role.");
            }

            // Validate role-specific fields
            if (model.Role == AppRoles.Instructor && string.IsNullOrWhiteSpace(model.InstructorTitle))
            {
                ModelState.AddModelError(nameof(model.InstructorTitle), "Instructor title is required.");
            }

            if (model.Role == AppRoles.Student && string.IsNullOrWhiteSpace(model.StudentNumber))
            {
                ModelState.AddModelError(nameof(model.StudentNumber), "Student number is required.");
            }

            if (!ModelState.IsValid)
            {
                model.RolesList = AppRoles.All.Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList();
                return View(model);
            }

            try
            {
                // Get external login info from the current request
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    TempData["ErrorMessage"] = "External login session expired. Please try again.";
                    return RedirectToAction(nameof(Login));
                }

                // Create the user
                var userName = await GenerateUniqueUserNameAsync(model.Email.Split('@')[0]);
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    CreatedAt = DateTime.Now
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    foreach (var error in createResult.Errors)
                        ModelState.AddModelError("", error.Description);
                    
                    model.RolesList = AppRoles.All.Select(r => new SelectListItem
                    {
                        Text = r,
                        Value = r
                    }).ToList();
                    return View(model);
                }

                // Add user to role
                await _userManager.AddToRoleAsync(user, model.Role);

                // Create domain entity based on selected role
                if (model.Role == AppRoles.Instructor)
                {
                    var instructor = new DataAccess.Entities.Instructor
                    {
                        UserId = user.Id,
                        Title = model.InstructorTitle
                    };
                    await _unitOfWork.Instructors.AddAsync(instructor);
                    await _unitOfWork.SaveAsync();
                }
                else if (model.Role == AppRoles.Student)
                {
                    var student = new DataAccess.Entities.Student
                    {
                        UserId = user.Id,
                        StudentNumber = model.StudentNumber
                    };
                    await _unitOfWork.Students.AddAsync(student);
                    await _unitOfWork.SaveAsync();
                }

                // Link external login using the original info
                var linkResult = await _userManager.AddLoginAsync(user, info);
                if (!linkResult.Succeeded)
                {
                    // If linking fails, clean up the user
                    await _userManager.DeleteAsync(user);
                    TempData["ErrorMessage"] = "Failed to link external login. Please try again.";
                    return RedirectToAction(nameof(Login));
                }

                // Sign in the user
                await _signInManager.SignInAsync(user, isPersistent: true);

                TempData["SuccessMessage"] = $"Registration completed successfully! Welcome to Quizzly!";

                // Use the centralized redirect method
                return RedirectToDashboardOrHome(user, null);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "An error occurred while completing your registration. Please try again.";
                model.RolesList = AppRoles.All.Select(r => new SelectListItem
                {
                    Text = r,
                    Value = r
                }).ToList();
                return View(model);
            }
        }

        private IActionResult RedirectToDashboardOrHome(ApplicationUser user, string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl) && returnUrl != "/")
                return LocalRedirect(returnUrl);

            if (_userManager.IsInRoleAsync(user, AppRoles.Admin).Result)
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });

            if (_userManager.IsInRoleAsync(user, AppRoles.Instructor).Result)
                return RedirectToAction("Index", "Dashboard", new { area = "Instructor" });
            
            if (_userManager.IsInRoleAsync(user, AppRoles.Student).Result)
                return RedirectToAction("Index", "Dashboard", new { area = "Student" });

            // Default fallback
            return RedirectToAction("Index", "Dashboard", new { area = "Student" });
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
            return View("~/Views/Shared/PrivacyPolicy.cshtml");
        }

        public IActionResult DataDeletion()
        {
            return View("~/Views/Shared/DataDeletion.cshtml");
        }

    }
}
