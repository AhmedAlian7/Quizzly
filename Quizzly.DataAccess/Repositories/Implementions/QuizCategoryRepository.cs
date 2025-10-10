using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizCategoryRepository : Repository<QuizCategory>, IQuizCategoryRepository
    {
        public QuizCategoryRepository(AppDbContext context) : base(context) { }


        public async Task<IEnumerable<QuizCategory>> GetAllByInstructorIdAsync(int instructorId)
        {
            return await _context.QuizCategories
                .Include(qc => qc.Quizzes)
                .Where(qc => qc.InstructorId == instructorId)
                .ToListAsync();
        }

        public async Task<int> GetTotalByInstructorIdAsync(int instructorId)
        {
            return await _context.QuizCategories
                .Where(qc => qc.InstructorId == instructorId)
                .CountAsync();
        }

    }
}
