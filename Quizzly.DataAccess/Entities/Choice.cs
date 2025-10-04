namespace Quizzly.DataAccess.Entities
{
    public class Choice : BaseEntity
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public int OrderIndex { get; set; }

        //Navigation
        public int QuestionId { get; set; }
        public Question Question { get; set; }

    }
}
