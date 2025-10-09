using Quizzly.Business.ViewModels.Analytics;

namespace Quizzly.Business.ViewModels.Student
{
    public class StudentDashboardDto
    {
        // Summary Statistics
        public int TotalQuizzesAttempted { get; set; }
        public int CompletedQuizzes { get; set; }
        public int ExitedQuizzes { get; set; }
        public int PendingGrading { get; set; }
        public decimal? AverageScore { get; set; }
        public decimal? BestScore { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }

        // Performance Analytics
        public List<StudentPerformanceDto> PerformanceOverTime { get; set; } = new();
        public List<QuizCategoryPerformanceDto> CategoryPerformance { get; set; } = new();
        public List<QuestionTypePerformanceDto> QuestionTypePerformance { get; set; } = new();

        // Recent Activity
        public List<StudentRecentAttemptDto> RecentAttempts { get; set; } = new();

        // Achievement Data
        public int ImprovementCount { get; set; } // Quizzes where score improved from previous attempt
        public decimal? AverageImprovement { get; set; }
    }

    public class StudentPerformanceDto
    {
        public DateTime Date { get; set; }
        public decimal? Score { get; set; }
        public string QuizTitle { get; set; }
        public int QuizId { get; set; }
    }

    public class QuizCategoryPerformanceDto
    {
        public string CategoryName { get; set; }
        public int Attempts { get; set; }
        public decimal? AverageScore { get; set; }
        public decimal? BestScore { get; set; }
    }

    public class QuestionTypePerformanceDto
    {
        public string QuestionType { get; set; }
        public int TotalQuestions { get; set; }
        public int CorrectAnswers { get; set; }
        public decimal Accuracy { get; set; }
    }

    public class StudentRecentAttemptDto
    {
        public int AttemptId { get; set; }
        public string QuizTitle { get; set; }
        public decimal? Score { get; set; }
        public string Status { get; set; }
        public DateTime AttemptDate { get; set; }
        public TimeSpan Duration { get; set; }
        public bool ShowScore { get; set; }
    }

}
