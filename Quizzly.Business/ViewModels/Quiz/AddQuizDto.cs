using Quizzly.Business.ViewModels.Question;
using System.ComponentModel.DataAnnotations;

namespace Quizzly.Business.ViewModels.Quiz
{
    public class AddQuizDto
    {
        [Required]
        [MaxLength(100)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        [Range(1, 180)]
        public int TimeLimit { get; set; }
        public bool ShuffleQuestions { get; set; } = false;
        public bool ShuffleChoices { get; set; } = false;
        public List<AddQuestionDto> addQuestionDtos { get; set; } = new List<AddQuestionDto>();

    }
}
