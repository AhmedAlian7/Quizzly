using Quizzly.DataAccess.Entities;

namespace Quizzly.Business.ViewModels.Instructor
{
    public class InstructorAllQuizzesDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Questions { get; set; }
        public QuizCategory? Category { get; set; }
        public int Attempts { get; set; }
        public decimal? AvgScore { get; set; }
        public int TimeLimit { get; set; }
    }
}
