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
            var avgTicks = _context.Quizzes
                .SelectMany(q => q.QuizAttempts)
                .Where(qa => qa.QuizId == quizId && qa.FinishedAt.HasValue)
                .Select(qa => new { StartedAt = qa.StartedAt, FinishedAt = qa.FinishedAt })
                .AsEnumerable()
                .Select(qa => (double)(qa.FinishedAt.Value - qa.StartedAt).Ticks)
                .DefaultIfEmpty(0)
                .Average();

            if (avgTicks <= 0)
                return TimeSpan.Zero;

            return TimeSpan.FromTicks((long)avgTicks);
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
                .Include(q => q.QuizAttempts)
                .Include(q => q.Questions)
                .ThenInclude(qn => qn.Choices)
                .Where(q => q.InstructorId ==InstructorId & !q.IsDeleted)
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

        public async Task<Quiz?> GetByAccessTokenAsync(string accessToken, string includes = "")
        {
            var query = _context.Quizzes.AsQueryable();
            if (!string.IsNullOrWhiteSpace(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }
            return await query.FirstOrDefaultAsync(q => q.AccessToken == accessToken);
        }
    }
}
