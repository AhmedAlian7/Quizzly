using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class InstructorRepository : Repository<Instructor>, IInstructorRepository
    {
        public InstructorRepository(AppDbContext context) : base(context) { }

        public async Task<Instructor?> GetByUserIdAsync(string userId)
        {
            return await _dbSet.
                FirstOrDefaultAsync(i => i.UserId == userId);
        }

    }
}
