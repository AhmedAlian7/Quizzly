using Microsoft.EntityFrameworkCore;
using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuestionRepository : Repository<Question>, IQuestionRepository
    {
        public QuestionRepository(AppDbContext context) : base(context) { }

        public IQueryable<Question> GetQueryable()
        {
            return _context.Questions.Include(q => q.Answers);
        }

    }
}
