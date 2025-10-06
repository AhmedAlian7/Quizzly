using Quizzly.Business.ViewModels.Question;

namespace Quizzly.Business.ViewModels.Quiz
{
    public class AddQuizDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public int TimeLimit { get; set; }
        public bool ShuffleQuestions { get; set; } = false;
        public bool ShuffleChoices { get; set; } = false;
        public List<AddQuestionDto> addQuestionDtos { get; set; } = new List<AddQuestionDto>();

    }
}
