using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IQuizAttemptRepository : IRepository<QuizAttempt>
    {
        IQueryable<QuizAttempt> GetQueryable(string includes = "");
        Task<int> CountCompletedAttemptsForStudentAsync(int quizId, int studentId);
        Task<List<QuizAttempt>> GetRecentAttemptsForStudentAsync(int studentId, int take, string includes = "");
        Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, string includes = "");
        Task<List<QuizAttempt>?> GetPending();

    }
}
