using cw5.DAO;
using cw5.DTO.Request;
using cw5.Exceptions;
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

            try
            {
                return Ok(_enrollmentsDbService.EnrollNewStudent(enrollmentStudentRequest));
            }
            catch (BadRequestException ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpPost]
        [Route("promotions")]
        public IActionResult PromoteStudents(PromoteStudentsRequest promoteStudentsRequest)
        {
            try
            {
                return Ok(_enrollmentsDbService.PromoteStudents(promoteStudentsRequest));
            }
            catch (BadRequestException ex)
            {
                return NotFound(ex.Message);
            }
        }
        
        
        [HttpGet]
        [Route("getEnrollmentByStudentIndexSqlInjectionVulnerable")]
        public IActionResult GetEnrollmentByStudentIndexSqlInjectionVulnerable(string indexNumber)
        {
            var enrollmentStudentResponse = 
                _enrollmentsDbService.GetEnrollmentByStudentIndexSqlInjectionVulnerable(indexNumber);

            if (enrollmentStudentResponse != null )
            {
                return Ok(enrollmentStudentResponse);
            }

            return NotFound($"Enrollment for Student with indexNumber = {indexNumber} not found");
        }
    }
}