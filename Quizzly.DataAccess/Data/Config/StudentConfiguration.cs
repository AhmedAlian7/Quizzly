using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.HasKey(s => s.Id);

            builder.Property(s => s.StudentNumber)
                .HasMaxLength(50);


            builder.HasMany(s => s.QuizAttempts)
                .WithOne(qa => qa.Student)
                .HasForeignKey(qa => qa.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
