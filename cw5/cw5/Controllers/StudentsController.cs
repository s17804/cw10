using cw5.DAO;
using Microsoft.AspNetCore.Mvc;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _studentsDbService;

        public StudentsController(IStudentsDbService studentsDbService)
        {
            _studentsDbService = studentsDbService;
        }

        [HttpGet]
        [Route("getAllStudents")]
        public IActionResult GetAllStudents(string studentIndex)
        {
            return Ok(_studentsDbService.GetAllStudents());
        }

        [HttpGet]
        [Route("getStudentByIndexNumberSqlInjectionInVulnerable")]
        public IActionResult GetStudentByIndexNumberSqlInjectionInVulnerable(string indexNumber)
        {
            var studentWithStudiesResponse =
                _studentsDbService.GetStudentByIndexNumberSqlInjectionInVulnerable(indexNumber);

            if (studentWithStudiesResponse != null)
            {
                return Ok(studentWithStudiesResponse);
            }

            return NotFound($"Student with indexNumber = {indexNumber} not found");
        }
    }
}