namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {

        IAnswerRepository AnswerRepository { get; }
        IChoiceRepository ChoiceRepository { get; }
        IQuestionRepository QuestionRepository { get; }
        IInstructorRepository InstructorRepository { get; }
        IQuizAttemptRepository QuizAttemptRepository { get; }
        IQuizRepository QuizRepository { get; }
        IQuizCategoryRepository QuizCategoryRepository { get; }
        IStudentInfoFieldRepository StudentInfoFieldRepository { get; }
        IStudentInfoResponseRepository StudentInfoResponseRepository { get; }

        Task<int> SaveAsync();
        //IRepository<TEntity> Repository<TEntity>() where TEntity : class;

    }
}
