using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace Quizzly.Business.ViewModels.Authentication
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot exceed 100 characters")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot exceed 100 characters")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }



        [Remote(action: "IsEmailInUse", controller: "Account", ErrorMessage = "Email is already Used ,Try different Email")]
        [EmailAddress]
        [Required]
        public string Email { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [MinLength(6, ErrorMessage = "Password Must be Bigger than 6 char")]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "Password Not Match")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = null!;


        [Required(ErrorMessage = "Please select a role")]
        [Display(Name = "Register as")]
        public string Role { get; set; } // "Instructor" or "Student"

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

        public IEnumerable<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? RolesList { get; set; } = new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();
    }
}
