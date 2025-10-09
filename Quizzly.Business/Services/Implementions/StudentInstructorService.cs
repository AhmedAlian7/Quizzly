using Quizzly.Business.Services.Interfaces;
using Quizzly.Business.ViewModels.Student;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.Business.Services.Implementions
{
    public class StudentInstructorService : IStudentInstructorService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StudentInstructorService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }


        public async Task<List<StudentsTableDto>> studentsTableDtos(int instructorId)
        {
            var students = await _unitOfWork.Students
                .GetStudentsByInstructorId(instructorId);

            var studentDtos = students.Select(s => new StudentsTableDto
            {
                FullName = $"{s.User.FirstName} {s.User.LastName}",
                Email = s.User.Email,
                
                QuizzesTaken = s.QuizAttempts?
                .Count(qa => qa.Quiz?.InstructorId == instructorId) ?? 0,
                
                AverageScore = s.QuizAttempts?
                .Where(qa => qa.Quiz?.InstructorId == instructorId && qa.Score.HasValue)
                .Select(qa => qa.Score.Value)
                .DefaultIfEmpty(0)
                .Average() ?? 0

            }).ToList();

            return studentDtos;
        }


    }
}
