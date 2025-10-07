using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizAttemptRepository : Repository<QuizAttempt>, IQuizAttemptRepository
    {
        public QuizAttemptRepository(AppDbContext context) : base(context) { }

        public IQueryable<QuizAttempt> GetQueryable()
        {
            return _context.QuizAttempts;
        }

    }
}
