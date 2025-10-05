namespace Quizzly.DataAccess.Entities
{
    public class Answer : BaseEntity
    {
        public string? TextAnswer { get; set; } 
        public decimal MaxPoints { get; set; } 
        public decimal? PointsAwarded { get; set; } // via manaual grading
        public bool? IsCorrect { get; set; }
        public bool IsGraded { get; set; }

        public DateTime? GradedAt { get; set; }

        //Navigation
        public int QuizAttemptId { get; set; }
        public QuizAttempt QuizAttempt { get; set; }
        
        public int QuestionId { get; set; }
        public Question Question { get; set; }
        
        public int? ChoiceId { get; set; }
        public Choice? Choice { get; set; }
    }
}
