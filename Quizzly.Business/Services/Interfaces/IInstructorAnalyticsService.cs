using Quizzly.Business.ViewModels.Analytics;

namespace Quizzly.Business.Services.Interfaces
{

    public interface IInstructorAnalyticsService
    {
        Task<int> GetTotalQuizzesAuthoredAsync(int instructorId);
        Task<decimal?> GetAverageQuizScoreAsync(int instructorId);
        Task<List<QuestionPerformanceDto>> GetQuestionLevelPerformanceAsync(int quizId);
        Task<List<CommonIncorrectAnswerDto>> GetCommonIncorrectAnswersAsync(int quizId);
        Task<TimeSpan> GetAverageQuizTimeAsync(int quizId);
        Task<List<StudentScoreDistributionDto>> GetStudentPerformanceDistributionAsync(int quizId , int numberOfRanges);
        Task<List<TopPreformingStudentDto>> GetTopPreformingStudentAsync(int InstructorId);
        Task<List<QuizPerformanceDto>> GetQuizPerformanceAsync(int instructorId);
        Task<int> GetTotalCategoriesByInstructorAsync(int instructorId);
    }


}
