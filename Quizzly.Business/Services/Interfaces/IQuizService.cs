using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDetailsDto> GetQuizByIdAsync(int quizId);
        Task UpdateQuizAsync(QuizDetailsDto dto);


    }
}
