using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class QuizAttemptConfiguration : IEntityTypeConfiguration<QuizAttempt>
    {
        public void Configure(EntityTypeBuilder<QuizAttempt> builder)
        {
            builder.HasQueryFilter(qa => !qa.IsDeleted);

            builder.HasKey(qa => qa.Id);

            builder.Property(qa => qa.AttemptNumber)
                .IsRequired();

            builder.Property(qa => qa.StudentIdentifier)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(qa => qa.StartedAt)
                .IsRequired();


            builder.Property(qa => qa.MaxScore)
                .IsRequired();

            builder.Property(qa => qa.IsCompleted)
                .IsRequired();

            builder.Property(qa => qa.IsAutoGraded)
                .IsRequired();

            builder.Property(qa => qa.IsPublished)
                .IsRequired();

            builder.Property(qa => qa.IpAddress)
                .IsRequired()
                .HasMaxLength(45); // IPv6 max length

            builder.Property(qa => qa.UserAgent)
                .HasMaxLength(600);

            // Relationship: QuizAttempt belongs to one Quiz
            builder.HasOne(qa => qa.Quiz)
                .WithMany(q => q.QuizAttempts)
                .HasForeignKey(qa => qa.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: QuizAttempt has many Answers
            builder.HasMany(qa => qa.Answers)
                .WithOne(a => a.QuizAttempt)
                .HasForeignKey(a => a.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: QuizAttempt has many StudentInfoResponses
            builder.HasMany(qa => qa.StudentInfoResponses)
                .WithOne(r => r.QuizAttempt)
                .HasForeignKey(r => r.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
