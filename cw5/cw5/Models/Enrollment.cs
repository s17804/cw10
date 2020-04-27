using System;

namespace cw5.Models
{
    public class Enrollment
    {
        public int IdEnrollment { get; set; }
        public int Semester { get; set; }
        public DateTime StartDate { get; set; }
        
        public Studies Studies { get; set; }
    }

}