using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IQuizRepository : IRepository<Quiz>
    {
        Task<int> GetTotalQuizzesPerInstructor(int InstructorId);
        Task<IEnumerable<Quiz>> GetAllByInstructorId(int InstructorId);
        Task<IEnumerable<Quiz>> GetRecentQuizzezPerInstructor(int InstructorId);
        Task<decimal?> GetAvgScore(int QuizId);
        Task<TimeSpan> GetAvgTime(int QuizId);
        Task<decimal?> GetAvgScorePerInstructor(int InstructorId);
        Task<int> GetTotalStudentsCountPerInstructor(int InstructorId);
        Task<Quiz?> GetByAccessTokenAsync(string accessToken, bool includeRelations = true);
    }
}
