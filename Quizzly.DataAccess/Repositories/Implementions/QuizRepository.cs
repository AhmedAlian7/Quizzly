using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizRepository : Repository<Quiz>, IQuizRepository
    {
        public QuizRepository(AppDbContext context) : base(context) { }


        public async Task<int> GetTotalQuizzes(int InstructorId)
        {
            return await _context.Quizzes
                .Where(q => q.InstructorId == InstructorId)
                .CountAsync();
        }
    }
}
