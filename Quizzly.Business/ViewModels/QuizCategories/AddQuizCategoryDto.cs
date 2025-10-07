using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.QuizCategories
{
    public class AddQuizCategoryDto
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; } = string.Empty;

    }
}
