using System;
using System.ComponentModel.DataAnnotations;

namespace cw5.DTO.Response
{
    public class EnrollmentStudentResponse
    {
        [Required] public string IndexNumber { get; set; }
        [Required] public int Semester { get; set; }
        [Required] public string StartDate { get; set; }
        [Required] public string StudiesName { get; set; }
    }
}