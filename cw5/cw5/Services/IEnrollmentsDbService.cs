using cw5.DTO.Request;
using cw5.DTO.Response;

namespace cw5.Services
{
    public interface IEnrollmentsDbService
    {
        EnrollmentStudentResponse GetEnrollmentByStudentIndexSqlInjectionVulnerable(string studentIndex);

        EnrollmentResponse EnrollNewStudent(EnrollmentStudentRequest enrollmentStudentRequest);

        EnrollmentResponse PromoteStudents(PromoteStudentsRequest promoteStudentsRequest);
    }
}