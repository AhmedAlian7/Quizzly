using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class QuizCategoryRepository : Repository<QuizCategory>, IQuizCategoryRepository
    {
        public QuizCategoryRepository(AppDbContext context) : base(context) { }

    }
}
