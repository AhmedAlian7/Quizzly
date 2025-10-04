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


            builder.HasIndex(q => q.AccessToken)
                .IsUnique();
            
            builder.Property(q => q.AccessToken)
                .IsRequired();


            builder.Property(q => q.Description)
                .HasMaxLength(500);

            builder.Property(q => q.Title)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(q => q.DurationMintes)
                .IsRequired();

            builder.Property(q => q.PassingScore)
                .HasMaxLength(50);

            builder.Property(q => q.ShuffleQuestions)
                .IsRequired();

            builder.Property(q => q.ShuffleChoices)
                .IsRequired();

            builder.Property(q => q.IsPublished)
                .IsRequired();

            builder.Property(q => q.IsAutoGraded)
                .IsRequired();

            builder.Property(q => q.AllowMultipleAttempts)
                .IsRequired();

            builder.Property(q => q.ShowCorrectAnswers)
                .IsRequired();

            builder.Property(q => q.ShowScoreImmediatlely)
                .IsRequired();

            builder.Property(q => q.MaxAttempts);

            
            
            // Configure relationships

            builder.HasOne<Instructor>()
                .WithMany(i => i.Quizzes) 
                .HasForeignKey(q => q.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<QuizCategory>()
                .WithMany(c => c.Quizzes) 
                .HasForeignKey(q => q.QuizCategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(q => q.Students)
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
