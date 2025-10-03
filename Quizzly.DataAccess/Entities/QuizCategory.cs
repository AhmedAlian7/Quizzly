namespace Quizzly.DataAccess.Entities
{
    public class QuizCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }

        //Navigation
        public int InstructorId { get; set; }



    }
}
