namespace Quizzly.Business.ViewModels.Instructor
{
    public class ManualGradingListDto
    {
        public int AttemptId { get; set; }
        public string QuizTitle { get; set; }
        public string StudentName { get; set; }
        public bool IsGraded { get; set; }
    }
}
