using lab_13;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;
using System.Runtime.ConstrainedExecution;
using System.Timers;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

//Lab ID 27
// Note i comment every part after exec it to make no dups but when i push these on github i uncomment all
namespace Lab14
{
    //Part A 
    // how many students the PreInit script reported, and how many migrations were already applied.  
    // 4 students were reported, and 2 migrations were already applied.


    //Part B
    //B1 : Prints 3.99. but nothing has been sent to the database yet
    //B2: prints 0 for every instructor because ToListAsync() loads only the Instructor table
    //It does not load related Courses
    //B3 :  Nothing is saved to the database because AsNoTracking() tells Entity Framework Core not to track
    //the entity it retrieves only modifies the object in memory 

    // Part C
    // GPA = 3.0 + ((27 % 7) * 0.1) = 3.0 + (6 * 0.1) = 3.6



    // Part D
    //What operation does Up perform on FullName?
    //Up performs: AlterColumn<string>

    //What are nullable: and oldNullable: set to, and what does the difference mean?
    //nullable: false, old nullable: false→ the column was not  nullable  this migration makes it not null but but in
    //real it should change it from nullble to not nullable 

    //What two kinds of existing row could make this migration fail?
    //rows with FullName is null, or rows where LEN(FullName) > 100





    //part E
    //keeping both would be a mistake two competing sources of truth for the same fact

    //List every operation Up performs.?
    //Up performs: AddColumn InstructorId  DropColumn AssignedCourseName  AddForeignKey CreateIndex

    //One of them destroys data.Which one, and what data?
    //DropColumn AssignedCourseName

    // AssignedCourseName had no database check  it will silently stored the same bad reference
    //  with no error


    //In a comment, answer: the Include version returned more rows from SQL Server than there are instructors. Explain why, and what EF did with the duplicates.
    //Include's sql returns more rows than there are instructors because the JOIN produces one row per (Instructor, Course)






    //Part F
    //Load all instructors without Include, and loop printing each one's name and Courses.Count. Record: the counts printed, and how many SQL queries the log shows.
    //every count prints 0


    internal class Program
    {
        static async Task Main(string[] args)
        {
            using (var context = new StudentPortalContext())
            {
                //GPA = 3.0 + ((27 % 7) * 0.1) = 3.0 + (6 * 0.1) = 3.6
                // part C
                //c1
                double myGpa = 3.6;
                var nada = await context.Students.FirstAsync(x => x.FullName == "Nada Samir");
                Console.WriteLine($"Nada current GPA: {nada.Gpa}");
                //c2
                nada.Gpa = myGpa;
                Console.WriteLine($"GPA unsaved: {nada.Gpa}");
                //still show the old value: SaveChangesAsync() hasnot run

                //c3
                await context.SaveChangesAsync();
                // EF updates ONLY Gpa because the ChangeTracker compares current values against the originalvalues
                // snapshot taken when the entity was loaded

                //c4
                var m = new Student
                {
                    FullName = "Muhamad Assem Ahmed",
                    YearOfStudy = 2,
                    Gpa = myGpa
                };
                Console.WriteLine($"Id before save: {m.Id}"); //0
                context.Students.Add(m);
                await context.SaveChangesAsync();
                Console.WriteLine($"Id after save: {m.Id}");


                ////c5
                m.YearOfStudy = 3;
                await context.SaveChangesAsync();
                Console.WriteLine($"year of study after update: {m.YearOfStudy}");

                ////c6
                context.Students.Remove(m);
                await context.SaveChangesAsync();

                // Remove() has no async version because it is purely in-memory: it just flips the entity's ChangeTracker state to Deleted





                // Part D
                try
                {
                    context.Students.Add(new Student
                    {
                        FullName = null,
                        YearOfStudy = 1,
                        Gpa = 3.6
                    });
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    Console.WriteLine($" {e.GetType().Name}   {e.InnerException?.Message}");
                }






                // Part E
                // 27 % 2 = 1 → SetNull → InstructorId must be int?

                var instructors = new List<Instructor>()
                {
                    new Instructor
                    {
                        FullName = "Hamdy",
                        YearsOfExperience = 10
                    },
                    new Instructor
                    {
                        FullName = "Mona",
                        YearsOfExperience = 5,

                    }
                };

                var courses = new List<Course>()
                {
                    new Course
                    {
                        CourseName = "Web Development Using .NET",
                        Credits = 5
                    },
                    new Course
                    {
                        CourseName = "Database Fundamentals",
                        Credits = 5
                    }
                };

                context.Instructors.AddRange(instructors);
                context.Courses.AddRange(courses);
                await context.SaveChangesAsync();


                var crs = await context.Courses.FirstAsync(c => c.CourseName == "Web Development Using .NET");
                var h = await context.Instructors.FirstAsync(i => i.FullName == "Hamdy");
                crs.InstructorId = h.Id;
                await context.SaveChangesAsync();
                Console.WriteLine($"crs instriuctor id {crs.InstructorId}");

                try
                {
                    context.Courses.Add(new Course { CourseName = "tt", InstructorId = 9999 });
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateException e)
                {
                    Console.WriteLine($"{e.InnerException?.Message}");

                }








                //part F
                // (27 % 3) + 2 = 0 + 2 = 2
                var h2 = await context.Instructors.FirstAsync(i => i.FullName == "Hamdy");
                context.Courses.Add(new Course { CourseName = $"php", Credits = 2, InstructorId = h2.Id });
                context.Courses.Add(new Course { CourseName = $"laravel", Credits = 3, InstructorId = h2.Id });
                await context.SaveChangesAsync();
                Console.WriteLine("Created 2 more courses");



                var ins = await context.Instructors.ToListAsync();
                foreach (var i in ins)
                    Console.WriteLine($"{i.FullName}: {i.Courses.Count}");
                //counts print 0 for every instructor 

                var ins2 = await context.Instructors.Include(i => i.Courses).ToListAsync();
                foreach (var i in ins2)
                {
                    Console.WriteLine($"{i.FullName}:{i.Courses.Count}");
                    foreach (var c in i.Courses)
                        Console.WriteLine($"  {c.CourseName}");

                }
                //real counts print, and the log shows exactly 1 query



                //Include's sql returns more rows than there are instructors because the JOIN produces one row per (Instructor, Course)








                var oneInstructor = await context.Instructors.FirstOrDefaultAsync();
                if (oneInstructor != null)
                {
                    Console.WriteLine($"Loaded {oneInstructor.FullName} , courses {oneInstructor.Courses.Count}");
                    // Some code ................

                    // explicit loading
                    await context.Entry(oneInstructor).Collection(i => i.Courses).LoadAsync();
                    Console.WriteLine($"After Loading {oneInstructor.FullName} , courses {oneInstructor.Courses.Count}");
                }




                var readOnlyStudents = await context.Students
                    .AsNoTracking()
                    .ToListAsync();
                readOnlyStudents[0].Gpa = 0.0;
                await context.SaveChangesAsync();

                //SSMS shows the value unchanged.AsNoTracking entities are never added to the ChangeTracker






                //Part G
                // Part C GPA = 3.0 + ((27 % 7) * 0.1) = 3.6
                // Part E OnDelete = 27 % 2 = 1 → SetNull(InstructorId-> int ?)
                // Part F extra courses = (27 % 3) + 2 = 2
                //OnDelete = SetNull requires the FK to be nullable
                // no migration failled
                //both come from re-hitting the database every time you touch data multiple enumeration re runs the whole linq
                //N+1 re-runs in each row query each time you touch a related

            }
        }
    }
}
