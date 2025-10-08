namespace Quizzly.Business.ViewModels.Student
{
    public class StudentAccessViewModel
    {
        public string AccessToken { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string InstructorName { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime? StartAt { get; set; }
        public DateTime? EndAt { get; set; }
        public bool IsPublished { get; set; }
        public bool AllowMultipleAttempts { get; set; }
        public int? MaxAttempts { get; set; }
        public bool AlreadyAttempted { get; set; }
        public string? ValidationMessage { get; set; }
    }
    public class ClientAnswerDto
    {
        public int questionId { get; set; }
        public int? choiceId { get; set; }
        public string? textAnswer { get; set; }
    }
}


