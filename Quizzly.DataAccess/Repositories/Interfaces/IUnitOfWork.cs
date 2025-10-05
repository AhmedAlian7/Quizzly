namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IAnswerRepository Answers { get; }
        IChoiceRepository Choices { get; }
        IQuestionRepository Questions { get; }
        IInstructorRepository Instructors { get; }
        IQuizAttemptRepository QuizAttempts { get; }
        IQuizRepository Quizzes { get; }
        IQuizCategoryRepository QuizCategories { get; }
        IStudentInfoFieldRepository StudentInfoFields { get; }
        IStudentInfoResponseRepository StudentInfoResponses { get; }

        Task<int> SaveAsync();
    }
}
