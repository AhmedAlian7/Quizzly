namespace Quizzly.DataAccess.Entities
{
    public class QuizCategory : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int InstructorId { get; set; } // FK to Instructor
        public Instructor Instructor { get; set; }
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
    }
}
