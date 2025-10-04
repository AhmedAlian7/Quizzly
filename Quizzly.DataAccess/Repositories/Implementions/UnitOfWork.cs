using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class UnitOfWork
    {
        private readonly AppDbContext _context;
        public UnitOfWork(AppDbContext context)
        {
            _context = context;

            Answers = new AnswerRepository(_context);
            Choices = new ChoiceRepository(_context);
            Questions = new QuestionRepository(_context);
            Instructors = new InstructorRepository(_context);
            QuizAttempts = new QuizAttemptRepository(_context);
            Quizs = new QuizRepository(_context);
            QuizCategorys = new QuizCategoryRepository(_context);
            StudentInfoFields = new StudentInfoFieldRepository(_context);
            StudentInfoResponses = new StudentInfoResponseRepository(_context);
        }

       public IAnswerRepository Answers { get; }
       public IChoiceRepository Choices { get; }
       public IQuestionRepository Questions { get; }
       public IInstructorRepository Instructors { get; }
       public IQuizAttemptRepository QuizAttempts { get; }
       public IQuizRepository Quizs { get; }
       public IQuizCategoryRepository QuizCategorys { get; }
       public IStudentInfoFieldRepository StudentInfoFields { get; }
       public IStudentInfoResponseRepository StudentInfoResponses { get; }


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
