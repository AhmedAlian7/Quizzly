using Quizzly.DataAccess.Data;
using Quizzly.DataAccess.Entities;
using Quizzly.DataAccess.Repositories.Interfaces;

namespace Quizzly.DataAccess.Repositories.Implementions
{
    public class StudentInfoResponseRepository : Repository<StudentInfoResponse>, IStudentInfoResponseRepository
    {
        public StudentInfoResponseRepository(AppDbContext context) : base(context) { }

    }
}
