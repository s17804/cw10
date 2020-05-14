using cw5.DTO.Request;
using cw5.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace cw5.Controllers
{
    [ApiController]
    [Route("api/students")]
    public class StudentsController : ControllerBase
    {
        private readonly IStudentsDbService _studentsDbService;
        private readonly IJwtLogInService _jwtLogInService;

        public StudentsController(IStudentsDbService studentsDbService, IJwtLogInService jwtLogInService)
        {
            _studentsDbService = studentsDbService;
            _jwtLogInService = jwtLogInService;
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

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(LoginRequestDto loginRequestDto)
        {
            var tokens = _jwtLogInService.LogIn(loginRequestDto);
            return Ok( new
            {
                token  = tokens.Token,
                refreshToken = tokens.RefreshToken
            });
        }
        
        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public IActionResult RefreshToken(RefreshTokenRequest refreshTokenRequest)
        {
            var tokens = _jwtLogInService.RefreshJwtToken(refreshTokenRequest);
            return Ok( new
            {
                token  = tokens.Token,
                refreshToken = tokens.RefreshToken
            });
        }
    }
}