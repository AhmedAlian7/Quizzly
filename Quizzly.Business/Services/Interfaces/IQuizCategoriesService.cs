using Quizzly.DataAccess.Entities;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IQuizCategoriesService
    {

        Task<IEnumerable<QuizCategory>> GetAllAsync();

    }
}
