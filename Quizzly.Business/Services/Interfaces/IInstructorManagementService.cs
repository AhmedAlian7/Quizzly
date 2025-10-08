using Quizzly.Business.ViewModels.Instructor;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.Business.ViewModels.QuizCategories;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IInstructorManagementService
    {

        Task<decimal?> GetAvgScoreAsync(int InstructorId);
        Task<int> GetTotalStudentsCountAsync(int InstructorId);
        Task<List<InstructorRecentQuizDto>> GetRecentQuizzesAsync(int InstructorId);
        Task<List<InstructorAllQuizzesDto>> GetAllQuizzesAsync(int InstructorId);
        Task<InstructorDashboardDto> GetInstructorDashboardAsync(int InstructorId);
        Task<int> AddQuizAsync(int instructorId, AddQuizDto dto);
        Task AddQuizCategoryAsync(int instructorId, AddQuizCategoryDto dto);
        Task<Instructor?> GetInstructorByUserIdAsync(string userId);



    }
}
