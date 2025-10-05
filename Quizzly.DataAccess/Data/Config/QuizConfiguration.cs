using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class QuizConfiguration : IEntityTypeConfiguration<Quiz>
    {
        public void Configure(EntityTypeBuilder<Quiz> builder)
        {
            builder.HasQueryFilter(q => !q.IsDeleted);
            builder.HasKey(q => q.Id);
            builder.HasIndex(q => q.AccessToken).IsUnique();
            builder.Property(q => q.AccessToken).IsRequired();
            builder.Property(q => q.Description).HasColumnType("nvarchar(1000)");
            builder.Property(q => q.Title).IsRequired().HasColumnType("nvarchar(200)");
            builder.Property(q => q.DurationMintes).IsRequired();
            builder.Property(q => q.PassingScore).HasColumnType("decimal(5,2)");
            builder.Property(q => q.ShuffleQuestions).IsRequired();
            builder.Property(q => q.ShuffleChoices).IsRequired();
            builder.Property(q => q.IsPublished).IsRequired();
            builder.Property(q => q.IsAutoGraded).IsRequired();
            builder.Property(q => q.AllowMultipleAttempts).IsRequired();
            builder.Property(q => q.ShowCorrectAnswers).IsRequired();
            builder.Property(q => q.ShowScoreImmediatlely).IsRequired();
            builder.Property(q => q.MaxAttempts);
            builder.Property(q => q.InstructorId).IsRequired();
            builder.HasOne(qz => qz.Instructor)
                .WithMany(i => i.Quizzes)
                .HasForeignKey(q => q.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(qz => qz.QuizCategory)
                .WithMany(c => c.Quizzes)
                .HasForeignKey(q => q.QuizCategoryId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasMany(q => q.StudentInfoFields)
                .WithOne(s => s.Quiz)
                .HasForeignKey(s => s.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(q => q.QuizAttempts)
                .WithOne(a => a.Quiz)
                .HasForeignKey(a => a.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(q => q.Questions)
                .WithOne(qn => qn.Quiz)
                .HasForeignKey(qn => qn.QuizId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
