using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class StudentInfoResponseConfiguration : IEntityTypeConfiguration<StudentInfoResponse>
    {
        public void Configure(EntityTypeBuilder<StudentInfoResponse> builder)
        {
            builder.HasQueryFilter(s => !s.IsDeleted);

            builder.HasKey(s => s.Id);

            builder.Property(s => s.ResponseValue)
                .IsRequired()
                .HasMaxLength(1000);

            // Relationship: StudentInfoResponse belongs to one StudentInfoField
            builder.HasOne(s => s.StudentInfoField)
                .WithMany()
                .HasForeignKey(s => s.StudentInfoFieldId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship: StudentInfoResponse belongs to one QuizAttempt
            builder.HasOne(s => s.QuizAttempt)
                .WithMany(qa => qa.StudentInfoResponses)
                .HasForeignKey(s => s.QuizAttemptId)
                .OnDelete(DeleteBehavior.Restrict);
        }


    }
}
