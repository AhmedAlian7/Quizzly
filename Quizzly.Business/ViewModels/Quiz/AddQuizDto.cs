using Microsoft.AspNetCore.Mvc.Rendering;
using Quizzly.Business.ViewModels.Question;
using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.Quiz
{
    public class AddQuizDto
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(1, 180)]
        public int TimeLimit { get; set; }
        public bool ShuffleQuestions { get; set; } = false;
        public bool ShuffleChoices { get; set; } = false;
        public bool AllowMultipleAttempts { get; set; } = false;
        public bool IsAutoGraded { get; set; }

        [Range(1,5)]
        public int? MaxAttempts { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShowScoreImmediatlely { get; set; }
        public decimal? PassingScore { get; set; }
        public DateTime StartAt { get; set; } = DateTime.UtcNow;
        public DateTime EndAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int CategoryId { get; set;}
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public List<AddQuestionDto> addQuestionDtos { get; set; } = new List<AddQuestionDto>();

    }
}
