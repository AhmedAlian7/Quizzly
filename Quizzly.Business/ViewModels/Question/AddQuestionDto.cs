using Quizzly.Business.ViewModels.Choice;
using Quizzly.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.Question
{
    public class AddQuestionDto
    {

        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        [Required]
        [Range(1, 100)]
        public int Points { get; set; }

        [Url]
        public string? ImageUrl { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }
        public List<AddChoiceDto> Choices { get; set; } = new List<AddChoiceDto>();

    }
}
