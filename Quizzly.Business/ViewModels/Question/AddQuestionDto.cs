using Microsoft.AspNetCore.Http;
using Quizzly.Business.ViewModels.Choice;
using Quizzly.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizzly.Business.ViewModels.Question
{
    public class AddQuestionDto
    {

        [Required]
        [MaxLength(500)]
        public string Text { get; set; }

        [Required]
        [Range(1, 100)]
        public decimal Points { get; set; }
        public bool IsRequired { get; set; } = true;

        [Url]
        [NotMapped]
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }

        [Required]
        public QuestionType QuestionType { get; set; }

        [MaxLength(500)]
        public string? CorrectAnswer { get; set; }
        public List<AddChoiceDto> Choices { get; set; } = new List<AddChoiceDto>();

    }
}
