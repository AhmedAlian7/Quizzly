using Quizzly.Business.ViewModels.Instructor;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class InstructorDashboardService
    {
        private readonly IUnitOfWork _unitOfWork;

        public InstructorDashboardService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<int> GetTotalQuizzesAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalQuizzesPerInstructor(InstructorId);
        }
        public async Task<decimal?> GetAvgScoreAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetAvgScorePerInstructor(InstructorId);
        }
        public async Task<int> GetTotalStudentsCountAsync(int InstructorId)
        {
            return await _unitOfWork.Quizzes
                .GetTotalStudentsCountPerInstructor(InstructorId);

        }
        public async Task<List<InstructorRecentQuizDto>> GetRecentQuizzesAsync(int InstructorId)
        {
            var RecentQuizzes = await _unitOfWork.Quizzes
                .GetRecentQuizzezPerInstructor(InstructorId);

            var recentQuizzesDto = RecentQuizzes.Select(q => new InstructorRecentQuizDto
            {
                Title = q.Title,
                Attempts = q.QuizAttempts.Count(),
                IsPublished = q.IsPublished,
                CreatedAt = q.CreatedAt,
                AvgScore = q.QuizAttempts
                          .Average(qa => qa.Score),
            }).ToList();

            return recentQuizzesDto;
        }

        public async Task<InstructorDashboardDto> GetInstructorDashboardAsync(int InstructorId)
        {
            return (new InstructorDashboardDto
            {
                 instructorRecentQuizDtos = await GetRecentQuizzesAsync(InstructorId),
                 AvgScore = await GetAvgScoreAsync(InstructorId),
                 TotalQuizzes = await GetTotalQuizzesAsync(InstructorId),
                 TotalStudents = await GetTotalStudentsCountAsync(InstructorId),
            });

        }




    }
}
