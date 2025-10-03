namespace Quizzly.DataAccess.Entities
{
    public class Answer : BaseEntity
    {
        public string? TextAnswer { get; set; } 
        public decimal MaxPoints { get; set; } 
        public decimal? PointsAwarded { get; set; } 
        public bool? IsCorrect { get; set; }
        public DateTime? GradedAt { get; set; }

        //Navigation
        public int QuizAttemptId { get; set; }
        public int QuestionId { get; set; }
        public int? ChoiceId { get; set; }
    }
}
