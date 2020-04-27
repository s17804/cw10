namespace cw5.Models
{
    public class Student
    {
        
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudiesName { get; set; }
        public string BirthDate { get; set; }
        public int Semester { get; set; }

        public override string ToString()
        {
            return FirstName + ", " + LastName + ", " + StudiesName + ", " + BirthDate + ", " + Semester;
        }
    }
}