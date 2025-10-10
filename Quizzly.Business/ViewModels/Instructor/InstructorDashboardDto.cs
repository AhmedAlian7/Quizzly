using Quizzly.Business.ViewModels.Analytics;
using Quizzly.Business.ViewModels.QuizCategories;

namespace Quizzly.Business.ViewModels.Instructor
{
    public class InstructorDashboardDto
    {
        public List<InstructorRecentQuizDto> instructorRecentQuizDtos { get; set; }
        public List<QuizPerformanceDto> quizPerformanceDtos { get; set; }
        public List<TopPreformingStudentDto> topPreformingStudentDtos { get; set; }
        public List<QuizCategoryStatsDto> quizCategoryStatsDtos { get; set; }
        public int TotalStudents { get; set; }
        public int TotalQuizzes { get; set; }
        public int TotalCategories { get; set; }
        public decimal? AvgScore { get; set; }

    }
}
