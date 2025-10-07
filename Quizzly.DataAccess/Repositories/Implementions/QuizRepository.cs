using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(AppDbContext context) : base(context) { }

        public async Task<int> GetTotalQuizzesPerInstructor(int InstructorId)
        {
            return await _context.Quizzes
                .Where(q => q.InstructorId == InstructorId)
                .CountAsync();
        }
        public async Task<decimal?> GetAvgScorePerInstructor(int InstructorId)
        {
            return await _context.Quizzes
                 .Where(q => q.InstructorId == InstructorId)
                 .SelectMany(q => q.QuizAttempts)
                 .AverageAsync(qa => qa.Score);
        }
        public async Task<decimal?> GetAvgScore(int QuizId)
        {
            return await _context.Quizzes
                .Where(q => q.Id == QuizId)
                .SelectMany(q => q.QuizAttempts)
                .AverageAsync(qa => qa.Score);
        }
        public async Task<TimeSpan> GetAvgTime(int quizId)
        {
            return TimeSpan.FromTicks(Convert.ToInt64(
                 await _context.Quizzes
                 .SelectMany(q => q.QuizAttempts)
                 .Where(qa => qa.QuizId == quizId && qa.FinishedAt.HasValue)
                 .Select(qa => (qa.FinishedAt.Value - qa.StartedAt).Ticks)
                 .DefaultIfEmpty(0)
                 .AverageAsync()
            ));
        }
        public async Task<int> GetTotalStudentsCountPerInstructor(int InstructorId)
        {
            return await _context.Quizzes
                .Where(q => q.InstructorId == InstructorId)
                .SelectMany(q => q.QuizAttempts)
                .CountAsync();
        }
        public async Task<IEnumerable<Quiz>> GetAllByInstructorId(int InstructorId)
        {
            return await _context.Quizzes
                .Where(q => q.InstructorId ==InstructorId)
                .ToListAsync();
                
        }
        public async Task<IEnumerable<Quiz>> GetRecentQuizzezPerInstructor(int InstructorId)
        {
            return await _context.Quizzes
                 .Include(q => q.QuizAttempts)
                 .Where(q => q.InstructorId == InstructorId)
                 .OrderByDescending(q => q.CreatedAt)
                 .Take(5)
                 .ToListAsync();
        }
    }
}
