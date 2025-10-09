using Quizzly.Business.ViewModels.Student;

namespace Quizzly.Business.Services.Interfaces
{
    public interface IStudentInstructorService
    {
        Task<List<StudentsTableDto>> studentsTableDtos(int instructorId);


    }
}
