using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Quizzly.Business.ViewModels.Authentication
{
    public class ExternalLoginCompletionViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = null!;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = null!;

        [EmailAddress]
        [Required]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Register as")]
        public string Role { get; set; } = null!; // "Instructor" or "Student"

        // Instructor-specific fields
        [StringLength(100, ErrorMessage = "Title cannot exceed 100 characters")]
        [Display(Name = "Title (e.g., Professor, Lecturer)")]
        public string? InstructorTitle { get; set; }

        // Student-specific fields
        [StringLength(50, ErrorMessage = "Student number cannot exceed 50 characters")]
        [Display(Name = "Student Number")]
        public string? StudentNumber { get; set; }

        [Display(Name = "I agree to the Terms and Conditions")]
        public bool AcceptTerms { get; set; }

        public IEnumerable<SelectListItem>? RolesList { get; set; } = new List<SelectListItem>();
        
        // Store external login info
        public string? ExternalLoginProvider { get; set; }
        public string? ExternalLoginProviderKey { get; set; }
    }
}
