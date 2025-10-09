namespace Quizzly.Business.ViewModels.Analytics
{
    public class TopPreformingStudentDto
    {
        public int StudentId { get; set; }
        public string StudentName { get; set; }
        public decimal? AvgScore { get; set; }
        public int Rank { get; set; }


    }
}
