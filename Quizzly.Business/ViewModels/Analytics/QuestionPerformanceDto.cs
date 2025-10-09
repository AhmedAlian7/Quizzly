

namespace Quizzly.Business.ViewModels.Analytics
{
    public class QuestionPerformanceDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public double PercentCorrect { get; set; } // Percentage of students who answered correctly.
        public double PercentIncorrect { get; set; } // Percentage of students who answered incorrectly.
        public int TotalAttempts { get; set; } // Number of student attempts on this question.
    }
}
