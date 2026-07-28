using System.Text.RegularExpressions;

namespace lab_13

    //Lab id: 27
    //Part C threshold = 2.5 + ((27 mod 4) * 0.3) = 2.5 + (3 * 0.3) = 2.5 + 0.9 = 3.4
    //Part D experience = (27 mod 5) + 3 = 2 + 3 = 5
    //Part G property = 27 mod 3 = 0 -> Email
{
    //part F
    //Which tables will its Up method create? Courses Students Instructors 
    //What column type did EF choose for Gpa, and what did it choose for FullName? float nvarchar(max)
    //Is FullName nullable? Did you write anything that told EF that? No FullName is "" defaulted
    //What would Down() do? Drop all three tables
    //I donot see ITI_StudentPortalDB_EF anywhere 
    //after running Update-Database i see it in SQL Server Explorer
    //Note two concrete differences in a comment. migration table is created and drop ledger table is created
    //part G
    //context.Students.Where(s => s.Gpa > 3.0).ToList() -> the Where is translated to SQL and runs ON the server
    //context.Students.ToList().Where(s => s.Gpa > 3.0)-> ToList() pulls EVERY row across the wire first, then the filtering
    //happens in memory
    //part I Add-Migration only writes a C# file describing a schema change while update make the tables in database 
    internal class Program
    {

        static void Main(string[] args)
        {
            
            List<Student> students = new List<Student>
            {
                new Student { FullName = "Yara Adel",    YearOfStudy = 2, Gpa = 3.5 },
                new Student { FullName = "Omar Hesham",  YearOfStudy = 3, Gpa = 2.8 },
                new Student { FullName = "Nada Samir",   YearOfStudy = 1, Gpa = 3.9 },
                new Student { FullName = "Kareem Fouad", YearOfStudy = 4, Gpa = 3.2 }
            };

            List<Instructor> instructors = new List<Instructor>
            {
                new Instructor { FullName = "Hamdy",       YearsOfExperience = 10,
                                 AssignedCourseName = "Web Development Using .NET" },
                new Instructor { FullName = "Mona Khalil", YearsOfExperience = 6,
                                 AssignedCourseName = "Database Fundamentals" }
            };

            List<Course> courses = new List<Course>
            {
                new Course { CourseName = "Web Development Using .NET", Credits = 4 },
                new Course { CourseName = "Database Fundamentals",      Credits = 3 }
            };

            // Session 12's chain, unchanged — proof the project runs
            // as-is, and the baseline today's EF half is compared against.
            Console.WriteLine("===== WARM-UP: Session 12's chain =====");
            var warmUp = students
                .Where(s => s.Gpa > 3.0)
                .OrderByDescending(s => s.Gpa)
                .Select(s => s.FullName)
                .ToList();
            foreach (string n in warmUp) Console.WriteLine($"  {n}");
            Console.WriteLine();
            double Threshold = 3.4;  // 2.5 + ((27 mod 4) * 0.3) = 2.5 + 0.9 = 3.4
            int total = students.Count();
            int above = students.Count(s => s.Gpa > Threshold);
            double avggpa = students.Average(s => s.Gpa);
            double high = students.Max(s => s.Gpa);
            double low = students.Min(s => s.Gpa);
            bool Below = students.Any(s => s.Gpa < 2.0);
            bool all = students.All(s => s.Gpa >= 2.0);
            Console.WriteLine("Part C1");
            //part c1
            Console.WriteLine($"Total: {total}");
            Console.WriteLine($"above threshold {above}");
            Console.WriteLine($"Average GPA: {avggpa:F2}");
            Console.WriteLine($"Highest GPA: {high}");
            Console.WriteLine($"Lowest GPA: {low}");
            Console.WriteLine($"Any below 2.0: {Below}");
            Console.WriteLine($"All at or above 2.0: {all}");
            //part c2,C3
            Console.WriteLine("Part C2");
            List<Student> empty = new List<Student>();
            Console.WriteLine($"Empty Count(): {empty.Count()}"); // 0, safe
            Console.WriteLine($"Empty Any(): {empty.Any()}");     // False, safe
            //double avg = empty.Average(s => s.Gpa);       //System.InvalidOperationException: Sequence contains no elements
            if (empty.Any())
            {
                Console.WriteLine($"average: {empty.Average(s => s.Gpa)}");
            }
            else
            {
                Console.WriteLine("list is empty");
            }


            //part c4
            Console.WriteLine("Grouped unsorted:");
            foreach (var g in students.GroupBy(s => s.YearOfStudy))
            {
                Console.WriteLine($"    Year {g.Key}  Count: {g.Count()}");
                foreach (var s in g) Console.WriteLine($"       {s.FullName}");
            }
            // not sorted. GroupBy preserves the order in which each distinct key is first saw while walking in list

            Console.WriteLine("Grouped gpa ");
            var gpaGroups = students.GroupBy(s => s.Gpa >= Threshold ? "perfect" : "not good enough");
            foreach (var g in gpaGroups)
            {
                Console.WriteLine($"    {g.Key}  Count: {g.Count()}");
                foreach (var s in g) Console.WriteLine($"       {s.FullName}");
            }
            Console.WriteLine("Grouped by YearOfStudy (sorted by key):");
            foreach (var g in students.GroupBy(s => s.YearOfStudy).OrderBy(g => g.Key))
            {
                Console.WriteLine($"  Year {g.Key} - Count: {g.Count()}");
                foreach (var s in g) Console.WriteLine($"      {s.FullName}");
            }

            Console.WriteLine("Grouped sorted :");
            foreach (var g in students.GroupBy(s => s.YearOfStudy).OrderBy(g => g.Key))
            {
                Console.WriteLine($"  Year {g.Key}  Count: {g.Count()}");
                foreach (var s in g) Console.WriteLine($"      {s.FullName}");
            }


            Console.WriteLine("Part D");
            int Exp = 5; // (27 mod 5) + 3 = 2 + 3 = 5

            var method = instructors.Join(
               courses,
               i => i.AssignedCourseName,
               c => c.CourseName,
               (i, c) => new { Instructor = i.FullName, Course = c.CourseName, c.Credits });

            Console.WriteLine("Join method:");

            foreach (var r in method)
                Console.WriteLine($"{r.Instructor} teach {r.Course} {r.Credits} credits");

            var Query = from i in instructors
                                  join c in courses on i.AssignedCourseName equals c.CourseName
                                  select new { Instructor = i.FullName, Course = c.CourseName, c.Credits };

            Console.WriteLine("Join query:");
            foreach (var r in Query)
                Console.WriteLine($"    {r.Instructor} teaches {r.Course} {r.Credits} credits");

            instructors.Add(new Instructor
            {
                FullName = "Muhamad Assem Ahmed",
                YearsOfExperience = Exp, 
                AssignedCourseName = "Machine Learning"
            });
            var afterAdd = instructors.Join(
                courses,
                i => i.AssignedCourseName,
                c => c.CourseName,
                (i, c) => new { Instructor = i.FullName, Course = c.CourseName, c.Credits }).ToList();

            Console.WriteLine($"Instructors in: {instructors.Count}");
            Console.WriteLine($"Rows out: {afterAdd.Count}");
            //An inner Join only prints a row when two sides have a matching key "Machine Learning" doesnot exist in courses
            //I will need a LEFT  join

            Console.WriteLine("Part E ");
            //the query is deferred Where doesn't run yet). Layla (Gpa 3.7 > 3.0) is added to the underlying list before the query is executed 
            //so when Count() finally runs it walks the list as it stands then   Layla should be included

            var deferred = students.Where(s => s.Gpa > 3.0);
            students.Add(new Student { FullName = "Layla Mostafa", YearOfStudy = 2, Gpa = 3.7 });
            int deferredCount = deferred.Count();
            Console.WriteLine($"count : {deferredCount}");

            students.RemoveAll(s => s.FullName == "Layla Mostafa");

            int repeat = 0;
            var bug = students.Where(s =>
            {
                repeat++;
                return s.Gpa > 3.0;
            });

            int c1 = bug.Count();         
            foreach (var s in bug) { }     
            double avg = bug.Average(s => s.Gpa); 
            Console.WriteLine($"repeated {repeat} times " + $"for {students.Count} students");

            var fixedd = students.Where(s => s.Gpa > 3.0).ToList();
            int Fixedcount = fixedd.Count();
            foreach (var s in fixedd) { }
            double avgFixed = fixedd.Average(s => s.Gpa);
            Console.WriteLine($"Fixed now runs once, all three operations reuse the same");

            var topS = students
                .MyTopStudents()
                .OrderBy(s => s.FullName)
                .Select(s => s.FullName)
                .ToList();

            Console.WriteLine("sorted Top:");
            foreach (var name in topS) Console.WriteLine($"{name}");
            //MyTopStudents() is deferred.Internally it's just a Where call nothing runs until something excute it







            using (var context = new StudentPortalContext())
            {
                if (!context.Students.Any())
                {
                    context.Students.AddRange(
                        new Student { FullName = "Yara Adel", YearOfStudy = 2, Gpa = 3.5 },
                        new Student { FullName = "Omar Hesham", YearOfStudy = 3, Gpa = 2.8 },
                        new Student { FullName = "Nada Samir", YearOfStudy = 1, Gpa = 3.9 },
                        new Student { FullName = "Kareem Fouad", YearOfStudy = 4, Gpa = 3.2 });
                    context.SaveChanges();
                }
                var Top = context.Students
                    .Where(s => s.Gpa > 3.0)
                    .OrderByDescending(s => s.Gpa)
                    .Select(s => s.FullName)
                    .ToList();
                Console.WriteLine("part H:");
                foreach (var t in Top) Console.WriteLine($"  {t}");
                Console.WriteLine($"database average GPA: {context.Students.Average(s => s.Gpa):F2}");
                Console.WriteLine($"database student count: {context.Students.Count()}");

                //context.Students.Where(s => s.Gpa > 3.0).ToList() -> the Where is translated to SQL and runs ON the server
                //context.Students.ToList().Where(s => s.Gpa > 3.0)-> ToList() pulls EVERY row across the wire first, then the filtering
                //happens in memory
            }
        }
    }

   
}
