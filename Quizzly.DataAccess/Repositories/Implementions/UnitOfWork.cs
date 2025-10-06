using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class UnitOfWork : IUnitOfWork   
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Answers = new AnswerRepository(_context);
            Choices = new ChoiceRepository(_context);
            Questions = new QuestionRepository(_context);
            Instructors = new InstructorRepository(_context);
            Students = new StudentRepository(_context);
            QuizAttempts = new QuizAttemptRepository(_context);
            Quizzes = new QuizRepository(_context);
            QuizCategories = new QuizCategoryRepository(_context);
            StudentInfoFields = new StudentInfoFieldRepository(_context);
            StudentInfoResponses = new StudentInfoResponseRepository(_context);
        }

       public IAnswerRepository Answers { get; private set; }
       public IChoiceRepository Choices { get; private set; }
       public IQuestionRepository Questions { get; private set; }
       public IInstructorRepository Instructors { get; private set; }
       public IStudentRepository Students { get; private set; }
       public IQuizAttemptRepository QuizAttempts { get; private set; }
       public IQuizRepository Quizzes { get; private set; }
       public IQuizCategoryRepository QuizCategories { get; private set; }
       public IStudentInfoFieldRepository StudentInfoFields { get; private set; }
       public IStudentInfoResponseRepository StudentInfoResponses { get; private set; }


        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
