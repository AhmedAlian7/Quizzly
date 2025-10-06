using Quizzly.Business.ViewModels.Instructor;
using Quizzly.Business.ViewModels.Quiz;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IInstructorManagementService
    {

        Task<int> GetTotalQuizzesAsync(int InstructorId);
        Task<decimal?> GetAvgScoreAsync(int InstructorId);
        Task<int> GetTotalStudentsCountAsync(int InstructorId);
        Task<List<InstructorRecentQuizDto>> GetRecentQuizzesAsync(int InstructorId);
        Task<List<InstructorAllQuizzesDto>> GetAllQuizzesAsync(int InstructorId);
        Task<InstructorDashboardDto> GetInstructorDashboardAsync(int InstructorId);
        Task AddQuizAsync(int instructorId, AddQuizDto dto);


    }
}
