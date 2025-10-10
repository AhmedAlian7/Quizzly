namespace Quizzly.Business.ViewModels.Instructor
{
    public class ManualAnswerDto
    {
        public int AnswerId { get; set; }              
        public string QuestionText { get; set; }     
        public string? StudentAnswer { get; set; }   
        public decimal MaxPoints { get; set; }       
        public decimal? PointsAwarded { get; set; }  
    }
}
