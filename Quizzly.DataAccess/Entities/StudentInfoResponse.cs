namespace Quizzly.DataAccess.Entities
{
    public class StudentInfoResponse : BaseEntity
    {
        public string ResponseValue { get; set; }

        //Navigation
        public int StudentInfoFieldId { get; set; }
        public int QuizAttemptId { get; set; }
    }
}
