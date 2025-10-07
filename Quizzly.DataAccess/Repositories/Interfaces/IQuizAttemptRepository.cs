using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IQuizAttemptRepository : IRepository<QuizAttempt>
    {

        IQueryable<QuizAttempt> GetQueryable();


    }
}
