
using Quizzly.Business.ViewModels.Analytics;

namespace Quizzly.Business.Services.Interfaces
{

    public interface IInstructorAnalyticsService
    {
        /// <summary>
        /// Returns the total number of quizzes authored by the instructor.
        /// </summary>
        Task<int> GetTotalQuizzesAuthoredAsync(int instructorId);

        /// <summary>
        /// Returns the average score achieved by students across all quizzes authored by the instructor.
        /// </summary>
        Task<decimal?> GetAverageQuizScoreAsync(int instructorId);

        /// <summary>
        /// Returns statistics for each question in a quiz, percent correct and incorrect.
        /// </summary>
        Task<List<QuestionPerformanceDto>> GetQuestionLevelPerformanceAsync(int quizId);

        /// <summary>
        /// Returns a list of the most common incorrect answers for each question in a quiz.
        /// </summary>
        Task<List<CommonIncorrectAnswerDto>> GetCommonIncorrectAnswersAsync(int quizId);

        /// <summary>
        /// Returns the average time students spend completing a quiz.
        /// </summary>
        Task<TimeSpan> GetAverageQuizTimeAsync(int quizId);

        /// <summary>
        /// Returns a distribution of student scores for a quiz (histogram data).
        /// </summary>
        Task<List<StudentScoreDistributionDto>> GetStudentPerformanceDistributionAsync(int quizId , int numberOfRanges);
        Task<List<TopPreformingStudentDto>> GetTopPreformingStudentAsync(int InstructorId);
        Task<List<QuizPerformanceDto>> GetQuizPerformanceAsync(int instructorId);
    }


}
