using Quizzly.DataAccess.Enums;

namespace Quizzly.DataAccess.Entities
{
    public class StudentInfoField : BaseEntity
    {
        public string FieldName { get; set; }
        public FieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public string? PlaceHolderText { get; set; }
        public string? ValidationRegex { get; set; }
        public string? DropdownOptions { get; set; } // JSON


        //Navigation
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public List<StudentInfoResponse> StudentInfoAnswers { get; set; } = new List<StudentInfoResponse>();
    }
}
