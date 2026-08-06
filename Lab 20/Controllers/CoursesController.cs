// =====================================================================
// CoursesController — CARRIED FORWARD FROM SESSION 19 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 20 — ASP.NET Core Razor Pages
//
// ⚠️ NOT ONE LINE OF THIS FILE CHANGES TODAY. Yesterday's other
//    direction — Course → Enrollments → Student — still works exactly
//    as it did, and Block 5 uses it as the control group.
// =====================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    public class CoursesController : Controller
    {
        private readonly StudentPortalContext _context;

        public CoursesController(StudentPortalContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int id)
        {
            var course = await _context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.Id == id);
            if (course is null)
                return NotFound();

            return View(course);
        }
    }
}