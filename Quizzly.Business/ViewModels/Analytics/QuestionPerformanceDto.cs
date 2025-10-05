

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

    public class CommonIncorrectAnswerDto
    {
        public int QuestionId { get; set; } 
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public int SelectionCount { get; set; } // Count of times this answer was wrongly selected.
    }

    public class StudentScoreDistributionDto
    {
        public double RangeStart { get; set; } // Lower bound of the score range.
        public double RangeEnd { get; set; } // Upper bound of the score range.
        public int StudentCount { get; set; } // Number of students within this score range.
    }
}
