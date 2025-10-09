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
    }
}


