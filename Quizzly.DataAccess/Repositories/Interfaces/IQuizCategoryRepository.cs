using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IQuizCategoryRepository : IRepository<QuizCategory>
    {

        Task<IEnumerable<QuizCategory>> GetAllByInstructorIdAsync(int instructorId);
        Task<int> GetTotalByInstructorIdAsync(int instructorId);

    }
}
