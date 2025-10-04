using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class StudentInfoFieldRepository : Repository<StudentInfoField>, IStudentInfoFieldRepository
    {
        public StudentInfoFieldRepository(AppDbContext context) : base(context) { }

    }
}
