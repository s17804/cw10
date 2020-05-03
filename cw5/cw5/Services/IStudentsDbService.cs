using System.Collections.Generic;
using cw5.DTO.Response;
using cw5.Models;

namespace cw5.Services
{
    public interface IStudentsDbService
    {
        IEnumerable<StudentWithStudiesResponse> GetAllStudents();

        StudentWithStudiesResponse GetStudentByIndexNumberSqlInjectionInVulnerable(string studentIndex);

        bool CheckIfStudentExists(string index);
    }
}