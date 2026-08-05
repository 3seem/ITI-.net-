//Part A

// LAB 19 — Lab ID: 27 | MIN_GRADE_LAB = 2.5 | COURSE_COUNT = 2
//Index only displays Enrollments.Count which just needs the Enrollment rows
//themselves loaded Details additionally prints each enrolled student's name, which is further

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;

namespace StudentPortalWeb.Controllers
{
    public class EnrollmentsController : Controller
    {
        private readonly StudentPortalContext _context;

        public EnrollmentsController(StudentPortalContext context)
        {
            _context = context;
        }


        //Part B
       //A new Student form has no dependency on existing data every field is typed fresh A new Enrollment is a
       // pairing between two EXISTING rows so the form can't offer real choices without
       // first loading the real Students and real Courses to pick from
       [HttpGet]
        public async Task<IActionResult> Create()
        {
            ViewData["Students"] = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Courses"] = await _context.Courses
                .OrderBy(c => c.CourseName)
                .ToListAsync();

            return View();
        }




        //A hidden field is still client-supplied data anyone can edit the HTML
        //or replay the request with a different value
        [HttpPost]

        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            //Console.WriteLine("POST reached");
            if (!ModelState.IsValid)
            {

                ViewData["Students"] = await _context.Students
                    .OrderBy(s => s.FullName)
                    .ToListAsync();
                ViewData["Courses"] = await _context.Courses
                    .OrderBy(c => c.CourseName)
                    .ToListAsync();

                return View(enrollment);
            }

            enrollment.EnrollmentDate = DateTime.Now;

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            var student = await _context.Students.FindAsync(enrollment.StudentId);
            var course = await _context.Courses.FindAsync(enrollment.CourseId);

            TempData["Message"] = $"Enrolled {student?.FullName} in {course?.CourseName}";

            return RedirectToAction("Details", "Students", new { id = enrollment.StudentId });
        }
    }
}