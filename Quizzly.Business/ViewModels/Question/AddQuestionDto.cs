using Quizzly.Business.ViewModels.Choice;
using Quizzly.DataAccess.Enums;

namespace Quizzly.Business.ViewModels.Question
{
    public class AddQuestionDto
    {
        public string Text { get; set; }
        public int Points { get; set; }
        public QuestionType QuestionType { get; set; }
        public List<AddChoiceDto> Choices { get; set; } = new List<AddChoiceDto>();

    }
}
