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

            builder.Property(qc => qc.InstructorId)
                .IsRequired();

            builder.HasOne(qc => qc.Instructor)
                .WithMany(i => i.QuizCategories)
                .HasForeignKey(qc => qc.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
