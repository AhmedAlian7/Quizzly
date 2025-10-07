using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IInstructorRepository : IRepository<Instructor>
    {
         Task<Instructor?> GetByUserIdAsync(string userId);
    }
}
