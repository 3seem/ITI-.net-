// LAB 16 — Lab ID: 27 | MAX_YEAR = 4 | MIN_GPA = 2.5 | INTAKE_CODE = itiA

// Part A
//The `default` route sits at the bottom because conventional routes are matched
//top-to-bottom, first match wins

//Part B
// It's acceptable for two URLs to reach the same action as long as each has a
// clear purpose


//Part C
//MAX_YEAR(4) IS accepted range(1, 4) is inclusive on both ends,
//so /students/top/4 matches; / students / top / 5
//fails the constraint and 404s before the action





//Part E
// / Students / About is a 404 because attribute routing replaces conventional routing


//minGpa belongs in the query string rather than the path because it's
// optional filter on the resource, not part of identifying theresource itself



//Part F
//Lab ID 27
// MAX_YEAR = (27 mod 4) + 1 = 3 + 1 = 4
// MIN_GPA = 2.5 + (27 mod 3) × 0.5 = 2.5 + 0 = 2.5
// INTAKE_CODE = 27 mod 3 = 0 → itiA


//Requesting /students/top/5: the request hits the routing middleware,
//which checks the pattern `students/top/{count:int:range(1, 4)}` The int
//constraint parses "5" successfully, but the range constraint rejects it
//because 5 > 4. Since no route matches, the request never reaches the
//controller/action



//Same: both the built-in `int`/`range` constraints and my `IntakeCodeConstraint`
//implement `IRouteConstraint` and are evaluated by the routing middleware in
//the same way — before any action executes


//It's a guarantee, not a limitation: once an action has its own [Route]
//attribute, ASP.NET Core stops offering the conventional {controller}/{ action}
//path for it
using lab_16.Constraints;
using lab_16.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using lab_16.Constraints;
using lab_16.Models;
using System;

namespace lab_16
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // =========================================================
            // PHASE ONE — WHAT CAN THIS APP DO?
            // Everything above builder.Build() registers capabilities
            // into the DI container. Nothing here handles a request yet.
            // =========================================================
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // TODO 1: ⚠️ Numbered 1 because it sits here physically, but
            //         TAUGHT FOURTH, in Block 4 — leave it blank until
            //         then. See the note at the top of this file for why
            //         it cannot move down next to the route it serves.
            //         Teach the routing system the nickname of the
            //         constraint class you will write in TODO 5. Call the
            //         routing configuration method on builder.Services,
            //         give it a lambda that receives an options object,
            //         and on that object's map of constraint nicknames,
            //         add one entry: the short lowercase word you want to
            //         type inside route patterns, paired with the type of
            //         your constraint class. Use the typeof operator for
            //         the second half — you are handing over the class
            //         itself, not an instance of it. The framework will
            //         create instances when it needs them, which is the
            //         same idea you met in Session 15.
            //         ⚠️ The nickname you choose here is the exact word
            //         TODO 4 must type after the colon. If the two ever
            //         disagree, the app throws at startup naming the
            //         constraint it could not find.

            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Data Source=.;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            });


            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));

                //Part D
                options.ConstraintMap.Add("intakecode", typeof(IntakeCodeConstraint));
            });

            var app = builder.Build();
            // ↑↑↑ THE DIVIDING LINE. Above: what exists. Below: what runs.

            // =========================================================
            // PHASE TWO — HOW IS A REQUEST HANDLED?
            // Every app.Use... call below adds one checkpoint to the
            // hallway a request walks down, in the order written.
            // =========================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Session 15, unchanged. Today this becomes a measuring
            // instrument: it prints the path of every request BEFORE
            // routing decides anything, so you can see the difference
            // between "the route rejected it" and "the action ran and
            // found nothing".
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"[START] Request path : {context.Request.Path}");
                await next.Invoke();
                Console.WriteLine($"[END] Request path : {context.Request.Path}");
            });

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            //app.UseAuthentication();
            app.UseAuthorization();

            // TODO 2: Add two custom routes here, ABOVE the default route
            //         that is already below you. Both are added with the
            //         same controller-route mapping method the default
            //         route uses, and both take three named arguments: a
            //         name, a pattern, and a defaults object.
            //         The first route: name it after the students list,
            //         give it the single literal segment students with no
            //         parameters at all, and in its defaults object state
            //         which controller and which action should answer it
            //         — the students controller and its listing action,
            //         written as plain strings without the word
            //         "Controller" on the end.
            //         The second route: name it after the student detail
            //         page, give it the literal segment students followed
            //         by one parameter segment holding the student's
            //         identifier, and default it to the same controller
            //         and the detail action.
            //         Notice what is NOT in either pattern: any mention
            //         of the controller or the action. In these routes
            //         the URL no longer describes your class names — the
            //         defaults object does, privately. That separation is
            //         the entire point of Block 2.

            app.MapControllerRoute(
                name: "roster",
                pattern: "roster",
                defaults: new { controller = "Students", action = "Index" });
            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" }
                );

            app.MapControllerRoute(
                name: "studentsDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" }
                );

            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" }
                );

            app.MapControllerRoute(
                name: "studentsHonours",
                pattern: "students/honours/{band:honourBand}",
                defaults: new { controller = "Students", action = "Honours" }
                );



            //Part C 
            app.MapControllerRoute(
                name: "topStudents",
                pattern: "students/top/{count:int:range(1,4)}",
                defaults: new { controller = "Students", action = "Top" });



            //Part D
            app.MapControllerRoute(
                name: "intakeStudents",
                pattern: "students/intake/{code:intakecode}",
                defaults: new { controller = "Students", action = "Intake" });


            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.MapControllers();

            // TODO 3: Two edits, both about refusing bad input at the
            //         door rather than crashing behind it.
            //         First, go back to TODO 2's detail route and attach
            //         the built-in whole-number constraint to its
            //         parameter, using a colon between the parameter name
            //         and the constraint name inside the same braces.
            //         Second, add a THIRD route above the default one:
            //         name it after the by-year listing, and give it the
            //         literal segment students, then the literal segment
            //         year, then one parameter segment for the academic
            //         year. Constrain that parameter twice in a row — to
            //         whole numbers, and to the inclusive numeric range
            //         one through four — by chaining both constraint
            //         names after the parameter with colons. Default it
            //         to the students controller and the by-year action.
            //         Predict before you run it: what should the browser
            //         show for a year of 7, and should the action run at
            //         all?

            // TODO 4: (Block 4 — do TODO 5 and TODO 1 first.) Add a
            //         fourth route above the default one. Pattern: the
            //         literal segment students, then the literal segment
            //         honours, then one parameter segment for the class
            //         band. Constrain that parameter with the nickname
            //         you registered in TODO 1 — the same colon syntax as
            //         the built-in constraints, because to the routing
            //         system there is no difference between the ones
            //         Microsoft wrote and the one you wrote. Default it
            //         to the students controller and the honours action.

            app.Run();
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Program.cs — Phase One (before builder.Build())
//   TODO 1: Register your constraint's nickname in the constraint map
//           [Block 4 — sits first only because it must run before Build]
//
// Program.cs — Phase Two (after builder.Build())
//   TODO 2: Custom routes for the students list and the student detail   [Block 2]
//   TODO 3: Constrain the detail id to integers; add the by-year route   [Block 3]
//   TODO 4: Add the honours route using your own constraint's nickname   [Block 4]
//
// Constraints/HonourBandConstraint.cs
//   TODO 5: Implement Match so only the three real band names pass       [Block 4]
//
// Controllers/StudentsController.cs
//   TODO 6: Give the search action its own address, and read the query   [Block 5]
// ---------------------------------------------------------------------
#endregion
