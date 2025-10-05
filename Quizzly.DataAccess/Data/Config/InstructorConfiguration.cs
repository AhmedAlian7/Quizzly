using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public partial class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.HasKey(i => i.Id);

            builder.Property(i => i.Title)
                .HasMaxLength(100);

            // Relationship: Instructor has many Quizzes
            builder.HasMany(i => i.Quizzes)
                .WithOne(q => q.Instructor)
                .HasForeignKey(q => q.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: Instructor has many QuizCategories
            builder.HasMany(i => i.QuizCategories)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
