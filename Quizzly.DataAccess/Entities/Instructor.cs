namespace Quizzly.DataAccess.Entities
{
    public  class Instructor : BaseEntity
    { 
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        
        //Navigation
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<QuizCategory> QuizCategories { get; set; } = new List<QuizCategory>();
    }
}
