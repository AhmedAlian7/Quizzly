using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class AnswerRepository : Repository<Answer>, IAnswerRepository
    {
        public AnswerRepository(AppDbContext context) : base(context) { }

        public IQueryable<Answer> GetQueryable()
        {
            return _context.Answers;
        }
    }
}
