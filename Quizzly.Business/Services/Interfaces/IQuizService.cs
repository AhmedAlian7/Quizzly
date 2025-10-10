using Quizzly.Business.ViewModels.Instructor;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Entities;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IQuizService
    {
        Task<QuizDetailsDto> GetQuizByIdAsync(int quizId);
        Task UpdateQuizAsync(QuizDetailsDto dto);
        Task<string> PublishQuizAsync(int quizId);
        Task DeleteQuizAsync(int quizId);
        Task<List<InstructorAllQuizzesDto>> GetQuizzesByCategory(int categoryId, int instructorId);

    }
}
