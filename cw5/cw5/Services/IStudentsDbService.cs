using System.Collections.Generic;
using cw5.DTO.Response;
using cw5.Models;

namespace cw5.DAO
{
    public interface IStudentsDbService
    {
        IEnumerable<StudentWithStudiesResponse> GetAllStudents();

        StudentWithStudiesResponse GetStudentByIndexNumberSqlInjectionInVulnerable(string studentIndex);
    }
}