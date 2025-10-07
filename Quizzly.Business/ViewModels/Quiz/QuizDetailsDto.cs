using Microsoft.AspNetCore.Mvc.Rendering;
using Quizzly.Business.ViewModels.Question;
using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.Quiz
{
    public class QuizDetailsDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeLimit { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleChoices { get; set; }
        public bool AllowMultipleAttempts { get; set; }
        public bool IsAutoGraded { get; set; }
        public int? MaxAttempts { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool IsPublished { get; set; }
        public bool ShowScoreImmediatlely { get; set; }
        public decimal? PassingScore { get; set; }
        public DateTime? StartAt { get; set; } = DateTime.UtcNow;
        public DateTime? EndAt { get; set; } = DateTime.UtcNow;
        public int CaregoryId { get; set; }
        public IEnumerable<SelectListItem>? Categories { get; set; }
        public List<QuestionDetailsDto> Questions { get; set; } = new List<QuestionDetailsDto>();

    }
}
