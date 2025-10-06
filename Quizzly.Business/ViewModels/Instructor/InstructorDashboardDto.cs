namespace Quizzly.Business.ViewModels.Instructor
{
    public class InstructorDashboardDto
    {
        public List<InstructorRecentQuizDto> instructorRecentQuizDtos { get; set; }
        public int TotalStudents { get; set; }
        public int TotalQuizzes { get; set; }
        public decimal? AvgScore { get; set; }

    }
}
