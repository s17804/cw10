using System;
using System.ComponentModel.DataAnnotations;

namespace cw5.DTO.Request
{
    public class EnrollmentStudentRequest
    {
        [Required] public string Index { get; set; }
        [Required] public string FirstName { get; set; }
        [Required] public string LastName { get; set; }
        [Required] public DateTime BirthDate { get; set; }
        [Required] public string Studies { get; set; }
    }
}