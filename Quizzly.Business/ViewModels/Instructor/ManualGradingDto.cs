namespace Quizzly.Business.ViewModels.Instructor
{
    public class ManualGradingDto // act like The Attempt of student
    {
        public int AttemptId { get; set; }         
        public string StudentName { get; set; }     
        public string QuizTitle { get; set; }    
        public List<ManualAnswerDto> Answers { get; set; } = new();
    }
}
