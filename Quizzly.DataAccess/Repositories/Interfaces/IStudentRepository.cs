using Quizzly.DataAccess.Entities;

namespace Quizzly.DataAccess.Repositories.Interfaces
{
    public interface IStudentRepository : IRepository<Student>
    {
        Task<Student?> GetByUserIdAsync(string userId, string includes = "");
        Task<IEnumerable<Student>> GetTopStudentsByInstructorIdAsync(int InstructorId);
        Task<IEnumerable<Student>> GetStudentsByInstructorId(int InstructorId);
    }
}


