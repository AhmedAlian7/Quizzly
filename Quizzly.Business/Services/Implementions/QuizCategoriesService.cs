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
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });

        }

        public async Task<AddQuizCategoryDto> GetDtoByIdAsync(int categoryId)
        {
            var category = await _unitOfWork.QuizCategories
                .GetByIdAsync(categoryId);
            
            if (category == null)
                return null;
            
            return new AddQuizCategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task UpdateQuizCategoryAsync(AddQuizCategoryDto addQuizCategoryDto)
        {
           var category = await _unitOfWork.QuizCategories
                .GetByIdAsync(addQuizCategoryDto.Id);

            category.Name = addQuizCategoryDto.Name;
            category.Description = addQuizCategoryDto.Description;
            category.UpdatedAt = DateTime.UtcNow;
            

            await _unitOfWork.SaveAsync();
        }


        public async Task DeleteQuizCategoryAsync(int categoryId)
        {
            var category = await _unitOfWork.QuizCategories
                .GetByIdAsync(categoryId);

            category.IsDeleted = true;

            await _unitOfWork.SaveAsync();

        }

    }
}
