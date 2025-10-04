namespace Quizzly.DataAccess.Entities
{
    public class QuizAttempt : BaseEntity
    {
        public int AttemptNumber { get; set; }
        public string StudentIdentifier { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? FinishedAt { get; set; }
        public decimal? Score { get; set; }
        public decimal MaxScore { get; set; }
        public decimal? Percentage { get; set; }
        public bool IsCompleted { get; set; }
        public bool IsAutoGraded { get; set; }
        public bool IsPublished { get; set; }
        public string IpAddress { get; set; }
        public string? UserAgent { get; set; }

        //Navigation
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
        public ICollection<StudentInfoResponse> StudentInfoResponses { get; set; } = new List<StudentInfoResponse>();
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
    }
}
