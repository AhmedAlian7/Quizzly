using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quizzly.DataAccess.Entities;
namespace Quizzly.DataAccess.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<QuizCategory> QuizCategories { get; set; }
        public DbSet<StudentInfoField> StudentInfoFields { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Choice> Choices { get; set; }
        public DbSet<Answer> Answers { get; set; }
        public DbSet<QuizAttempt> QuizAttempts { get; set; }
        public DbSet<StudentInfoResponse> StudentInfoResponses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
