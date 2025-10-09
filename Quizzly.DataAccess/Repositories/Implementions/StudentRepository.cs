using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class StudentRepository : Repository<Student>, IStudentRepository
    {
        public StudentRepository(AppDbContext context) : base(context) { }

        public async Task<Student?> GetByUserIdAsync(string userId, string includes = "")
        {
            var query = _context.Students.AsQueryable();
            if (!string.IsNullOrWhiteSpace(includes))
            {
                foreach (var include in includes.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(include.Trim());
                }
            }
            return await query.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Student>> GetTopStudentsByInstructorIdAsync(int instructorId)
        {
            return await _context.Students
                 .Include(s => s.User)
                 .Where(s => s.QuizAttempts.Any(qa => qa.Quiz.InstructorId == instructorId))
                 .Select(s => new
                 {
                     Student = s,
                     AverageScore = s.QuizAttempts
                         .Where(qa => qa.Quiz.InstructorId == instructorId)
                         .Average(qa => (decimal?)qa.Score) ?? 0
                 })
                 .OrderByDescending(s => s.AverageScore)
                 .Take(5)
                 .Select(s => s.Student)
                 .ToListAsync();
        }
    }
}


