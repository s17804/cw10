using cw5.DTO.Request;
using cw5.Services;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/enrollment")]
    public class EnrollmentController : ControllerBase
    {
        private readonly IEnrollmentsDbService _enrollmentsDbService;

        public EnrollmentController(IEnrollmentsDbService enrollmentsDbService)
        {
            _enrollmentsDbService = enrollmentsDbService;
        }

        [HttpPost]
        [Route("enrollNewStudent")]
        public IActionResult EnrollNewStudent(EnrollmentStudentRequest enrollmentStudentRequest)
        {
            return Ok(_enrollmentsDbService.EnrollNewStudent(enrollmentStudentRequest));
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            return Ok(_enrollmentsDbService.PromoteStudents(promoteStudentsRequest));
        }


        [HttpGet]
        [Route("getEnrollmentByStudentIndexSqlInjectionVulnerable")]
        public IActionResult GetEnrollmentByStudentIndexSqlInjectionVulnerable(string indexNumber)
        {
            var enrollmentStudentResponse =
                _enrollmentsDbService.GetEnrollmentByStudentIndexSqlInjectionVulnerable(indexNumber);

            if (enrollmentStudentResponse != null)
            {
                return Ok(enrollmentStudentResponse);
            }

            return NotFound($"Enrollment for Student with indexNumber = {indexNumber} not found");
        }
    }
}