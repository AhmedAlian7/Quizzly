using Microsoft.AspNetCore.Mvc;
using Quizzly.Business.Services.Interfaces;

namespace Quizzly.Web.Controllers
{
    public class TestController : Controller
    {
        private readonly IEmailService _emailService;

        public TestController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        public IActionResult SendEmail()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendEmail(string toEmail, string subject, string message)
        {
            try
            {
                if (string.IsNullOrEmpty(toEmail) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(message))
                {
                    TempData["ErrorMessage"] = "All fields are required.";
                    return View();
                }

                await _emailService.SendEmailAsync(toEmail, subject, message);
                TempData["SuccessMessage"] = "Email sent successfully!";
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Failed to send email. Please try again.";
                return View();
            }
        }

        // Example: Send welcome email
        public async Task<IActionResult> SendWelcomeEmail(string userEmail, string userName)
        {
            try
            {
                string subject = "Welcome to Our Application!";
                string body = $@"
                    <h2>Hello {userName}!</h2>
                    <p>Thank you for registering with our application.</p>
                    <p>We're excited to have you on board.</p>
                    <br/>
                    <p>Best regards,<br/>The Team</p>
                ";

                await _emailService.SendEmailAsync(userEmail, subject, body);
                return Ok("Welcome email sent successfully");
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to send welcome email");
            }
        }
    }
}

