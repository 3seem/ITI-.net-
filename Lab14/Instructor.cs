namespace lab_13
{
    public class Instructor
    {
        public int Id { get; set; }
        public string FullName { get; set; } = "";
        public int YearsOfExperience { get; set; }
        //public string? AssignedCourseName { get; set; }
        public List<Course> Courses { get; set; } = new();

    }

    
}
