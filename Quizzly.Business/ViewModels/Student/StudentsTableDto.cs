namespace Quizzly.Business.ViewModels.Student
{
    public class StudentsTableDto
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int QuizzesTaken { get; set; }
        public decimal? AverageScore { get; set; }

    }
}
