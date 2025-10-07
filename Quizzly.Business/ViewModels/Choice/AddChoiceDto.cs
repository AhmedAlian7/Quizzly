using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.Choice
{
    public class AddChoiceDto
    {
        [Required]
        [MaxLength(200)]
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
    }
}
