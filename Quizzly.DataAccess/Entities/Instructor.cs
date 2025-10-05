using System;

namespace Quizzly.DataAccess.Entities
{
    public class Instructor : BaseEntity
    {
        public string UserId { get; set; } // FK to ApplicationUser.Id
        public ApplicationUser User { get; set; }
        public string? Title { get; set; }
        public ICollection<Quiz> Quizzes { get; set; } = new List<Quiz>();
        public ICollection<QuizCategory> QuizCategories { get; set; } = new List<QuizCategory>();
    }
}
