namespace Quizzly.Business.ViewModels.Student
{
    public class QuizResultViewModel
    {
        public int QuizId { get; set; }
        public string QuizTitle { get; set; }
        public int AttemptId { get; set; }
        public decimal? Score { get; set; }
        public decimal MaxScore { get; set; }
        public decimal? Percentage { get; set; }
        public TimeSpan TimeTaken { get; set; }
        public bool Passed { get; set; }
        public bool IsAutoGraded { get; set; }
        public bool AwaitingManualGrading { get; set; }
        public bool ShowCorrectAnswers { get; set; }
        public bool ShowScoreImmediately { get; set; }

        // Auto-graded summary (MCQ & True/False only)
        public decimal AutoGradedMaxScore { get; set; }
        public decimal AutoGradedScore { get; set; }
        public decimal? AutoGradedPercentage { get; set; }

        public List<QuestionResultVm> Questions { get; set; } = new();

        public class QuestionResultVm
        {
            public int QuestionId { get; set; }
            public string Text { get; set; }
            public decimal Points { get; set; }
            public decimal? PointsAwarded { get; set; }
            public bool? IsCorrect { get; set; }
            public string? Explanation { get; set; }
            public string? Feedback { get; set; } // AI grading feedback
            public List<ChoiceResultVm> Choices { get; set; } = new();
            public List<int> SelectedChoiceIds { get; set; } = new();
            public string? TextAnswer { get; set; }
        }

        public class ChoiceResultVm
        {
            public int ChoiceId { get; set; }
            public string Text { get; set; }
            public bool IsCorrect { get; set; }
        }
    }
}


