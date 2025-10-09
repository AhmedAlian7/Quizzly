using Quizzly.Business.ViewModels.Analytics;

namespace Quizzly.Business.ViewModels.Instructor
{
    public class InstructorDashboardDto
    {
        public List<InstructorRecentQuizDto> instructorRecentQuizDtos { get; set; }
        public List<QuizPerformanceDto> quizPerformanceDtos { get; set; }
        public List<TopPreformingStudentDto> topPreformingStudentDtos { get; set; }
        public int TotalStudents { get; set; }
        public int TotalQuizzes { get; set; }
        public decimal? AvgScore { get; set; }

    }
}
