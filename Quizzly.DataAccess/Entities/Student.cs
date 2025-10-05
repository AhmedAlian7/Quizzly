using System;

namespace Quizzly.DataAccess.Entities
{
    public class Student : BaseEntity
    {
        public string UserId { get; set; } // FK to ApplicationUser.Id
        public ApplicationUser User { get; set; }
        public string? StudentNumber { get; set; }
        public ICollection<QuizAttempt> QuizAttempts { get; set; } = new List<QuizAttempt>();
    }
}
