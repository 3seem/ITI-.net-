// =====================================================================
// StudentsController — CARRIED FORWARD FROM SESSIONS 15-19 (Rule 39)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 20 — ASP.NET Core Razor Pages
//
// ⚠️ NOT ONE LINE OF THIS FILE CHANGES TODAY.
//
//    Keep this file open in a tab all morning. By Block 5 you will have
//    a Razor Page showing the SAME student, from the SAME database,
//    through the SAME DbContext — and this file will still be byte for
//    byte what it was when you walked in. That is the whole payoff, and
//    it only lands if nobody edits this file.
// =====================================================================
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentPortalWeb.Controllers
{
    public class StudentsController : Controller
    {
        private readonly StudentPortalContext _context;

        public StudentsController(StudentPortalContext context)
        {
            _context = context;
        }

        public IActionResult Demo(int id)
        {
            if (id == 0) return NotFound();
            if (id == 1) return Content("I'm a plain text , not a page.");
            if (id == 2) return Json(new { Message = "This is a JSON" , Id = id });
            if (id == 3) return RedirectToAction("Index");
            return View();
        }

        public IActionResult Echo(
            [FromRoute] int id,
            [FromQuery] string note,
            [FromHeader(Name = "User-Agent")] string agent
            )
        {
            return Content($"id (route) = {id} | note (query) = {note} | agent (header) = {agent}");
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student)
        {
            if (!ModelState.IsValid)
            {
                return View(student);
            }
            await _context.Students.AddAsync(student);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{student.FullName} was Added";

            return RedirectToAction("Index");
        }

        // Answers the students list route.
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        // Answers the student detail route.
        public async Task<IActionResult> Details(int id)
        
        {
            var student = await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                return NotFound();
            }

            return View(student);
        }

        // Answers the by-year route.
        public async Task<IActionResult> ByYear(int year)
        {
            var students = await _context.Students
                .Where(s => s.YearOfStudy == year)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Year"] = year;

            return View(students);
        }

        // Answers the honours route, guarded by the Session 16 constraint.
        public async Task<IActionResult> Honours(string band)
        {
            if (string.IsNullOrWhiteSpace(band))
            {
                return NotFound();
            }

            IQueryable<Student> query = _context.Students;

            if (string.Equals(band, "first", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.5);
            }
            else if (string.Equals(band, "second", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(s => s.Gpa >= 3.0 && s.Gpa < 3.5);
            }
            else
            {
                query = query.Where(s => s.Gpa < 3.0);
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Band"] = band.ToLowerInvariant();

            return View(students);
        }

        [Route("students/search")]
        public async Task<IActionResult> Searching([FromQuery] string name)
        {
            IQueryable<Student> query = _context.Students;

            if (!string.IsNullOrWhiteSpace(name))
            {
                query = query.Where(s => s.FullName.Contains(name));
            }

            var students = await query
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Name"] = name;

            return View(students);
        }
    }
}