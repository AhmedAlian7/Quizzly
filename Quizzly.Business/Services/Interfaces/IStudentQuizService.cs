using Quizzly.Business.ViewModels.Student;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IStudentQuizService
    {
        Task<QuizTakingViewModel> GetTakeViewModelAsync(int attemptId, int? questionId, int index);
        Task<int> StartAttemptAsync(string token, string userId, string userEmail, string? ipAddress);
        Task<int> SubmitAsync(int attemptId, string? answersJson);
        Task<QuizResultViewModel> GetResultAsync(int attemptId);
        Task<StudentAccessViewModel> GetAccessLinkAsync(string token, string userId);
        Task<List<DataAccess.Entities.QuizAttempt>> GetRecentAttemptsForStudentAsync(int studentId, int take);
        Task<List<DataAccess.Entities.QuizAttempt>> GetRecentAttemptsForUserAsync(string userId, int take);
    }
}


