

namespace Quizzly.Business.ViewModels.Analytics
{
    public class CommonIncorrectAnswerDto
    {
        public int QuestionId { get; set; } 
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }
        public int SelectionCount { get; set; } // Count of times this answer was wrongly selected.
    }
}
