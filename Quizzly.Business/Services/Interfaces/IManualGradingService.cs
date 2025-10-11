using Quizzly.DataAccess.Entities;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IManualGradingService
    {
        Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, string Include = "");
        Task<List<Answer>> GetAnswersNeedingManualGradingAsync(int attemptId);
        Task ManualGradeAnswerAsync(int answerId, decimal pointsAwarded);
        Task UpdateAttemptTotalScoreAsync(int attemptId);
        Task<bool> AreAllAnswersGradedAsync(int attemptId);
        Task<List<QuizAttempt>> GetPendingAttemptsAsync();
        Task CompleteManualGradingAsync(int attemptId);

    }
}
