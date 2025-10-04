using System.ComponentModel.DataAnnotations.Schema;

namespace Quizzly.DataAccess.Entities
{
    public class Quiz : BaseEntity
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int DurationMintes { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool ShuffleQuestions { get; set; }
        public bool ShuffleChoices { get; set; }
        public bool IsPublished { get; set; }
        public bool IsAutoGraded { get; set; }
        public bool AllowMultipleAttempts { get; set; }    
        public int? MaxAttempts { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShowScoreImmediatlely { get; set; }
        public string AccessToken { get; set; }
        public decimal? PassingScore { get; set; } // total score needed to pass the quiz

        // Navigation
        public int InstructorId { get; set; }
        public Instructor Instructor { get; set; }
        public int QuizCategoryId { get; set; }
        public QuizCategory QuizCategory { get; set; }
        public ICollection<StudentInfoField> Students { get; set; } = new List<StudentInfoField>();
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
        public ICollection<Question> Questions { get; set; } = new List<Question>();

    }
}
