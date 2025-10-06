namespace Quizzly.Business.ViewModels.Instructor
{
    public class InstructorRecentQuizDto
    {

        public string Title { get; set; }
        public bool IsPublished { get; set; }
        public int Attempts { get; set; }
        public decimal? AvgScore { get; set; }
        public DateTime CreatedAt { get; set; }


    }
}
