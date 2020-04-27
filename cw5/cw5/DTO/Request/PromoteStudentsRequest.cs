using System.ComponentModel.DataAnnotations;

namespace cw5.DTO.Request
{
    public class PromoteStudentsRequest
    {
       [Required] public string Studies { get; set; }
        [Required] public int Semester { get; set; }
    }
}