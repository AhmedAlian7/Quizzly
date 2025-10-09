using System;
using System.Collections.Generic;

namespace Quizzly.Business.ViewModels.Student
{
    public class StudentResultsOverviewViewModel
    {
        public decimal AverageScorePercentage { get; set; }
        public int CompletedQuizzesCount { get; set; }
        public decimal? BestScorePercentage { get; set; }
        public TimeSpan TotalTimeSpent { get; set; }

        public List<RecentAttemptItem> RecentAttempts { get; set; } = new();

        public class RecentAttemptItem
        {
            public int AttemptId { get; set; }
            public int QuizId { get; set; }
            public string QuizTitle { get; set; }
            public decimal? Percentage { get; set; }
            public int QuestionsCount { get; set; }
            public TimeSpan Duration { get; set; }
            public DateTime? FinishedAt { get; set; }
            public bool IsCompleted { get; set; }
            public string Status { get; set; }
            public DateTime DisplayAt { get; set; }
            public bool ShowScoreImmediately { get; set; }
        }
    }
}


