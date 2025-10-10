using Microsoft.AspNetCore.Http;
using Quizzly.Business.ViewModels.Choice;
using Quizzly.DataAccess.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quizzly.Business.ViewModels.Question
{
    public class QuestionDetailsDto
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public decimal Points { get; set; }
        public bool IsRequired { get; set; }
        public bool AutoGrade { get; set; } = false;
        public string? ImageUrl { get; set; }
        public IFormFile? ImageFile { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<AddChoiceDto> Choices { get; set; } = new List<AddChoiceDto>();

    }
}
