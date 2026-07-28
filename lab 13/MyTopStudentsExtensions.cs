namespace lab_13
{
   
    public static class MyTopStudentsExtensions
    {
        // Lab ID 27: 2.5 + ((27 mod 4) * 0.3) = 3.4
        private static double Threshold = 3.4;

        public static IEnumerable<Student> MyTopStudents(this IEnumerable<Student> source)
        {
            return source.Where(s => s.Gpa >= Threshold);
        }
    }
}
