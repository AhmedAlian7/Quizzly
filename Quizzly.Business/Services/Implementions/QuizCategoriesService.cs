using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.QuizCategories;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class QuizCategoriesService : IQuizCategoriesService
    {
        private readonly IUnitOfWork _unitOfWork;

        public QuizCategoriesService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<QuizCategory>> GetAllAsync()
        {
            return await _unitOfWork.QuizCategories
                .GetAllAsync("");
        }
        public async Task<IEnumerable<AddQuizCategoryDto>> GetAllByInstructorIdAsync(int InstructorId)
        {
            var categories = await _unitOfWork.QuizCategories
                .GetAllByInstructorIdAsync(InstructorId);

            return categories.Select(c => new AddQuizCategoryDto
            {
                Name = c.Name,
                Description = c.Description
            });

        }

    }
}
