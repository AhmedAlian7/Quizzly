using Quizzly.DataAccess.Enums;

namespace Quizzly.Business.ViewModels.Student
{
    public class QuizTakingViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int AttemptId { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? EndsAtUtc { get; set; }

        public int CurrentIndex { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionVm> Questions { get; set; } = new();

        public class QuestionVm
        {
            public int QuestionId { get; set; }
            public int OrderIndex { get; set; }
            public string Text { get; set; }
            public QuestionType QuestionType { get; set; }
            public bool IsRequired { get; set; }
            public decimal Points { get; set; }
            public string? Explanation { get; set; }
            public List<ChoiceVm> Choices { get; set; } = new();
            public string? ExistingTextAnswer { get; set; }
            public List<int> ExistingChoiceIds { get; set; } = new();
        }

        public class ChoiceVm
        {
            public int ChoiceId { get; set; }
            public string Text { get; set; }
        }
    }
}


