using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Data.Config
{
    public class StudentInfoFieldConfiguration : IEntityTypeConfiguration<StudentInfoField>
    {
        public void Configure(EntityTypeBuilder<StudentInfoField> builder)
        {
              builder.HasQueryFilter(qa => !qa.IsDeleted);
              
              builder.HasKey(s => s.Id);
              
              builder.Property(s => s.FieldName)
                  .IsRequired()
                  .HasMaxLength(200);
              
              builder.Property(s => s.FieldType)
                  .IsRequired();
              
              builder.Property(s => s.IsRequired)
                  .IsRequired();
              
              builder.Property(s => s.PlaceHolderText)
                  .HasMaxLength(500);
              
              builder.Property(s => s.ValidationRegex)
                  .HasMaxLength(500);
              
              builder.Property(s => s.DropdownOptions)
                  .HasMaxLength(1000);
              
              // Relationship: StudentInfoField belongs to one Quiz
              builder.HasOne(s => s.Quiz)
                  .WithMany(q => q.StudentInfoFields)
                  .HasForeignKey(s => s.QuizId)
                  .OnDelete(DeleteBehavior.Cascade);


        }

    }
}
