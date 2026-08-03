// LAB 17 — Lab ID: 27 | MIN_GPA_EDIT = 2.6 | MAX_YEAR_EDIT = 2

using lab_17.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using lab_17.Constraints;
using lab_17.Models;
using System;

namespace StudentPortalWeb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // =========================================================
            // PHASE ONE — WHAT CAN THIS APP DO?
            // =========================================================
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            // Session 16 — your own route constraint, still registered.
            builder.Services.AddRouting(options =>
            {
                options.ConstraintMap.Add("honourBand", typeof(HonourBandConstraint));
            });

            // Session 15, plus the SQL logging added in Session 16 so the
            // console shows every query Entity Framework really sends.
            // Today that log is the proof for the session's biggest claim:
            // when validation rejects a form, NO INSERT appears here.
            builder.Services.AddDbContext<StudentPortalContext>(options =>
            {
                options.UseSqlServer("Data Source=.;Initial Catalog=ITI_StudentPortal;Integrated Security=True;Encrypt=True;TrustServerCertificate=True")
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging();
            });

            var app = builder.Build();
            // ↑↑↑ THE DIVIDING LINE. Above: what exists. Below: what runs.

            // =========================================================
            // PHASE TWO — HOW IS A REQUEST HANDLED?
            // =========================================================
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            // Session 15's middleware. Session 16 used it to tell two
            // identical 404s apart. Today it does a third job: it shows
            // you that ONE click on a Save button produces TWO separate
            // requests, which is the whole subject of Block 5.
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

            // =========================================================
            // THE ROUTE TABLE — Session 16's work, finished and left
            // alone. Read it top to bottom: this is still the whole
            // public address surface of the application.
            // =========================================================
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

            app.Run();
        }
    }
}

#region 📋 Full TODO Checklist
// ---------------------------------------------------------------------
// Nothing in Program.cs today. Routing finished yesterday.
//
// Controllers/StudentsController.cs
//   TODO 1: One action that answers with four different kinds of result  [Block 1]
//   TODO 2: One action that proves where each parameter came from        [Block 2]
//   TODO 3: The empty form — the GET half of Create                      [Block 3]
//   TODO 4: The POST half of Create, and the attribute that marks it     [Block 3]
//   TODO 6: Refuse to save when the submitted data breaks the rules      [Block 4]
//   TODO 7: Save, then redirect instead of rendering                     [Block 5]
//
// Models/StudentPortalContext.cs
//   TODO 5: Add the validation rules the form will be checked against    [Block 4]
// ---------------------------------------------------------------------
#endregion
