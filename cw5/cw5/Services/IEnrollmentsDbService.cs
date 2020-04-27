using cw5.DTO.Request;
using cw5.DTO.Response;
using cw5.Models;

namespace cw5.DAO
{
    public interface IEnrollmentsDbService
    {
        EnrollmentStudentResponse GetEnrollmentByStudentIndexSqlInjectionVulnerable(string studentIndex);

        EnrollmentResponse EnrollNewStudent(EnrollmentStudentRequest enrollmentStudentRequest);

        EnrollmentResponse PromoteStudents(PromoteStudentsRequest promoteStudentsRequest);
    }
}