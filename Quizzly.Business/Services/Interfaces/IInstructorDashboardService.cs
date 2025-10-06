using Quizzly.Business.ViewModels.Instructor;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IInstructorDashboardService
    {

        Task<int> GetTotalQuizzesAsync(int InstructorId);
        Task<decimal> GetAvgScoreAsync(int InstructorId);
        Task<int> GetTotalStudentsCountAsync(int InstructorId);
        Task<List<InstructorRecentQuizDto>> GetRecentQuizzesAsync(int InstructorId);
        Task<InstructorDashboardDto> GetInstructorDashboard(int InstructorId);
        

    }
}
