using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class AnswerConfiguration : IEntityTypeConfiguration<Answer>
    {
        public void Configure(EntityTypeBuilder<Answer> builder)
        {
            builder.HasQueryFilter(a => !a.IsDeleted);

            builder.HasKey(a => a.Id);

            builder.Property(a => a.TextAnswer)
                .HasMaxLength(2000);

            builder.Property(a => a.MaxPoints)
                .IsRequired();

            builder.Property(a => a.IsCorrect)
                .IsRequired();

            builder.Property(a => a.Feedback)
                .HasMaxLength(1000);


            // Configure foreign key relationship to Question entity
            builder.HasOne(a => a.Question)
                .WithMany(q => q.Answers)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(a => a.QuizAttempt)
                .WithMany(q => q.Answers)
                .HasForeignKey(q => q.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
