using Quizzly.Business.Services.Interfaces;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services
{
    public class InstructorAnalyticsService : IInstructorAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorAnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalQuizzesAuthoredAsync(int instructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalQuizzes(instructorId);

        }
    }
}
