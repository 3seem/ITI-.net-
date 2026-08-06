// =====================================================================
// StudentPortalWeb — SESSION PROJECT (Style Guide Rule 20/34/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 20 — ASP.NET Core Razor Pages
//
// THIS PROJECT IS DAY-READY (Rule 39). Press F5 right now, before a
// single TODO is done, and the whole site from Sessions 15-19 works:
// /students, /students/3, /students/year/2, /students/honours/first,
// /Courses and /Courses/Details/1 all load real rows from the real
// ITI_StudentPortal database.
//
// ⚠️ TODAY IS THE FIRST SESSION SINCE 15 THAT CHANGES THIS FILE — and
//    it changes exactly TWO lines. That number is not a coincidence and
//    it is not a boast: Block 5 asks the room to count it out loud.
//
// ⚠️ NO MIGRATION TODAY. No new entity, no new column, no schema change
//    of any kind. Razor Pages is a way of SERVING data. It has nothing
//    to say about what the data is.
// =====================================================================
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using StudentPortalWeb.Constraints;
using StudentPortalWeb.Models;
using System;

namespace StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            // TODO 1 (part one): (Block 1.) One line, directly under the
            //         AddControllersWithViews line above it, on the
            //         BUILDER side of this file — before builder.Build().

            builder.Services.AddRazorPages();
            //
            //         Call the services method that registers everything
            //         the Razor Pages feature needs: the page-model
            //         activator, the page-route conventions, the page
            //         result executor. Its name is the word Add followed
            //         by the feature's name, no arguments.
            //
            //         ⚠️ Notice what you are NOT deleting. The line above
            //         stays. Both features are being registered into the
            //         SAME service container, in the SAME app. Nothing is
            //         being replaced today.
            //
            //         ⚠️ Miss this line and TODO 1 part two throws at
            //         startup with a message naming the missing service —
            //         one of the few genuinely loud failures today.
            builder.Services.AddRouting(options =>
                        {
                            options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
                        });

            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Data Source=.;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            });

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

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

            // ---- Sessions 16-19's four custom routes. UNCHANGED TODAY. ----
            app.MapControllerRoute(
                name: "studentsList",
                pattern: "students",
                defaults: new { controller = "Students", action = "Index" });

            app.MapControllerRoute(
                name: "studentsDetails",
                pattern: "students/{id:int}",
                defaults: new { controller = "Students", action = "Details" });

            app.MapControllerRoute(
                name: "studentsByYear",
                pattern: "students/year/{year:int:range(1,4)}",
                defaults: new { controller = "Students", action = "ByYear" });

            app.MapControllerRoute(
                name: "studentsHonours",
                pattern: "students/honours/{band:honourBand}",
                defaults: new { controller = "Students", action = "Honours" });

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");
            // TODO 1 (part two): (Block 1.) One more line, here — AFTER
            //         all five MapControllerRoute calls above, on the APP
            //         side of this file.

            app.MapRazorPages();
            //
            //         Call the app method that adds every Razor Page in
            //         the project to the same endpoint table those five
            //         routes just filled. Its name is the word Map
            //         followed by the feature's name, no arguments.
            //
            //         ⚠️ There is no pattern string. You do not tell it
            //         where the pages are or what URLs they answer. That
            //         is the entire difference you are about to see: a
            //         controller route is a pattern you WRITE, a Razor
            //         Page route is a file path it READS off the disk.
            //
            //         ⚠️ It is deliberately placed after the controller
            //         routes so you can see with your own eyes that
            //         adding it changes nothing about them. Block 5
            //         re-runs all four to prove it.
            app.Run();
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Program.cs                          (this file)
//   TODO 1: AddRazorPages + MapRazorPages — two parts        [Block 1]
//
// Pages/Hello.cshtml.cs               (already here, empty)
//   TODO 2: A public property and an OnGet that fills it     [Block 1]
// Pages/Hello.cshtml                  (YOU CREATE THIS FILE)
//   TODO 3: @page, @model, print the property                [Block 1]
//
// Pages/Roster/Index.cshtml.cs        (already here, empty)
//   TODO 4: Injected context, Students property, OnGetAsync  [Block 2]
// Pages/Roster/Index.cshtml           (YOU CREATE THIS FILE)
//   TODO 5: @page, @model, the roster table + <gpa-badge>    [Block 2]
//
// Pages/_ViewStart.cshtml             (YOU CREATE THIS FILE)
//   TODO 6: Give every page in Pages/ a layout               [Block 3]
// Pages/_ViewImports.cshtml           (YOU CREATE THIS FILE)
//   TODO 7: Usings + BOTH addTagHelper lines                 [Block 3]
//
// Pages/Roster/Details.cshtml.cs      (already here, empty)
//   TODO 8: OnGetAsync(id) + Include/ThenInclude + NotFound  [Block 4]
// Pages/Roster/Details.cshtml         (YOU CREATE THIS FILE)
//   TODO 9: @page "{id:int}", the enrollments table          [Block 4]
//
// Block 5 types nothing. Block 5 is where you find out what all of
// the above cost you in yesterday's files. Count them.
// ---------------------------------------------------------------------
#endregion
