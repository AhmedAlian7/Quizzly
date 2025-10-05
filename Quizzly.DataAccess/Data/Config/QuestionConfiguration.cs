using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasQueryFilter(q => !q.IsDeleted);

            builder.HasKey(q => q.Id);

            builder.Property(q => q.Text)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(q => q.ImageUrl)
                .HasMaxLength(500);

            builder.Property(q => q.QuestionType)
                .IsRequired();

            builder.Property(q => q.Points)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(q => q.OrderIndex)
                .IsRequired();

            builder.Property(q => q.ShuffleChoices)
                .IsRequired();

            builder.Property(q => q.ShowFeedback)
                .IsRequired();

            builder.Property(q => q.Explanation)
                .HasMaxLength(1000);

            // Relationship: Question belongs to one Quiz
            builder.HasOne(q => q.Quiz)
                .WithMany(quiz => quiz.Questions)
                .HasForeignKey(q => q.QuizId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Question has many Choices
            builder.HasMany(q => q.Choices)
                .WithOne(c => c.Question)
                .HasForeignKey(c => c.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Question has many Answers
            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
