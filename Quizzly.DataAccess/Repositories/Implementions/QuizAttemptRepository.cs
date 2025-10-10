using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizAttemptRepository : Repository<QuizAttempt>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(AppDbContext context) : base(context) { }

        public IQueryable<QuizAttempt> GetQueryable(string includes = "")
        {
            var query = _context.QuizAttempts.AsQueryable();
            if (!string.IsNullOrWhiteSpace(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }
            return query;
        }

        public async Task<int> CountCompletedAttemptsForStudentAsync(int quizId, int studentId)
        {
            return await _context.QuizAttempts
                .Where(a => a.QuizId == quizId && a.StudentId == studentId && a.IsCompleted)
                .CountAsync();
        }

        public async Task<List<QuizAttempt>> GetRecentAttemptsForStudentAsync(int studentId, int take, string includes = "")
        {
            var query = GetQueryable(includes)
                .Where(a => a.StudentId == studentId)
                .OrderByDescending(a => a.CreatedAt)
                .Take(take);
            return await query.ToListAsync();
        }

        public async Task<List<QuizAttempt>?> GetPending()
        {
            return await _context.QuizAttempts
                .Include(a => a.Quiz)
                .Include(a => a.Student)
                    .ThenInclude(s => s.User)
                .Include(a => a.Answers)
                    .ThenInclude(ans => ans.Question)
                .Where(a => a.Answers.Any(ans => ans.PointsAwarded == null))
                .ToListAsync();
        }


        public async Task<QuizAttempt?> GetAttemptByIdAsync(int attemptId, string includes = "")
        {
            return await GetQueryable(includes).FirstOrDefaultAsync(a => a.Id == attemptId);
        }
    }
}
