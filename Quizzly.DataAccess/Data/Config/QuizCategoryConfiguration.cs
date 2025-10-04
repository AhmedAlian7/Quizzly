using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class QuizCategoryConfiguration : IEntityTypeConfiguration<QuizCategory>
    {
        public void Configure(EntityTypeBuilder<QuizCategory> builder)
        {
            builder.HasQueryFilter(qc => !qc.IsDeleted);

            builder.HasKey(qc => qc.Id);

            builder.Property(qc => qc.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(qc => qc.Description)
                .HasMaxLength(1000);

            // Relationship: QuizCategory belongs to one Instructor
            builder.HasOne(qc => qc.Instructor)
                .WithMany(i => i.QuizCategories)
                .HasForeignKey(qc => qc.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: QuizCategory has many Quizzes
            //builder.HasMany(qc => qc.Quizzes)
            //    .WithOne(q => q.QuizCategory)
            //    .HasForeignKey(q => q.QuizCategoryId)
            //    .OnDelete(DeleteBehavior.Restrict);


        }

    }
}
