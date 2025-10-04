using Quizzly.DataAccess.Enums;

namespace Quizzly.DataAccess.Entities
{
    public class Question : BaseEntity
    {

        public string Text { get; set; }
        public string? ImageUrl { get; set; }
        public QuestionType QuestionType { get; set; }
        public int Order { get; set; }
        public bool ShuffleChoices { get; set; }
        public bool ShowFeedback { get; set; }
        public string? Explanation { get; set; }

        //Navigation
        public int QuizId { get; set; }
        public Quiz Quiz { get; set; }
        public ICollection<Choice> Choices { get; set; } = new List<Choice>();
        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
