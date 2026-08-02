// =====================================================================
// StudentsController — SESSION PROJECT (Rule 20/34/35/39/40)
// ITI Summer Training | Web Development Using .NET | Morning Group
// Session 16 — Routing
//
// ⚠️ WE ARE STILL NOT LEARNING CONTROLLERS TODAY.
//    Session 15 borrowed HomeController to PROVE that Dependency
//    Injection had delivered something real. Today we borrow this one
//    for the same kind of reason: a route has to point AT something, and
//    an empty room proves nothing. Session 17 is the session that builds
//    controllers properly — action results, model binding, validation,
//    all of it.
//
// Everything in this file is carried-forward skill (Rule 39): a
// constructor-injected context from Session 15, and EF Core queries from
// Session 14. It is all real, working code, pre-typed, and it compiles
// and runs before a single TODO is done. Not one line of it is today's
// topic.
//
// TODAY'S TOPIC IS THE URL, NOT THE METHOD. Every action below is
// currently reachable only through the default route inherited from
// Session 15 — /Students/Details/3 and friends. By the end of Block 4
// the same methods will answer /students/3, /students/year/2 and
// /students/honours/first, without one line inside them changing.
// =====================================================================

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using lab_16.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace lab_16.Controllers
{
    public class StudentsController : Controller
    {
        // Session 15, unchanged: the field, the constructor, the
        // injection. Nobody writes `new StudentPortalContext()` here.
        private readonly StudentPortalContext _context;

        public StudentsController(StudentPortalContext context)
        {
            _context = context;
        }

        // Answers the students list route (TODO 2).
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();

            return View(students);
        }

        // Answers the student detail route (TODO 2, constrained in TODO 3).
        //
        // The `id` parameter is filled in from ROUTE DATA — the routing
        // system pulls the value out of the URL segment and hands it to
        // this method by matching the segment's name to the parameter's
        // name. Rename either one and the value silently becomes 0.
        public async Task<IActionResult> Details(int id)
        {
            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.Id == id);

            if (student is null)
            {
                // The route matched, the action ran, the row did not
                // exist. That is a genuinely different failure from a URL
                // the route table refused, and Block 3 makes you tell
                // them apart from the console log alone.
                return NotFound();
            }

            return View(student);
        }

        // Answers the by-year route (TODO 3).
        public async Task<IActionResult> ByYear(int year)
        {
            var students = await _context.Students
                .Where(s => s.YearOfStudy == year)
                .OrderBy(s => s.FullName)
                .ToListAsync();

            ViewData["Year"] = year;

            return View(students);
        }

        // Answers the honours route (TODO 4), guarded by the constraint
        // you write in TODO 5.
        //
        // Read the guard order carefully. Reached through the HONOURS
        // route, `band` is already guaranteed to be one of three words,
        // because the constraint refused everything else — so the final
        // `else` below is not a silent catch-all for garbage; the garbage
        // never got this far. But this action is ALSO reachable through
        // the inherited default route, as /Students/Honours, where no
        // constraint applies at all. Hence the guard clause first.
        public async Task<IActionResult> Honours(string band)
        {
            // Guard clause before work — and note that this is NOT redundant
            // with the route constraint, even though the constraint makes it
            // unreachable through the honours route. The default route can
            // still reach this action as /Students/Honours with no band at
            // all, and an action must never assume it was only ever called
            // the way you intended. Constraints filter URLs; guard clauses
            // protect behaviour. Both, always.
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

        // TODO 6: Two changes to the method directly below, both about
        //         the OTHER way a URL carries information.
        //         First, give this action its own address, written on the
        //         action itself instead of in Program.cs. Put an
        //         attribute directly above the method whose name is the
        //         word Route, and pass it one string: the literal path
        //         students slash search, with no leading slash and no
        //         parameter segments at all. From that moment this action
        //         is reachable at exactly that address and — this is the
        //         part people get wrong — NO LONGER reachable through the
        //         default route, because an action that carries its own
        //         address stops accepting the conventional one.
        //         Second, mark the parameter below with the attribute
        //         that states plainly it comes from the query string
        //         rather than from a route segment. The code works
        //         without it; you are adding it so the method's signature
        //         tells the truth about where its data comes from, which
        //         is the difference between code that runs and code a
        //         colleague can read.
        //         Predict before you run it: after this change, what does
        //         /Students/Search?name=nada do?
        [Route("students/search")]
        public async Task<IActionResult> Searching([FromQuery] string name)
        {
            IQueryable<Student> query = _context.Students;

            // Guard clause before work: an empty search box is not an
            // error, it just means "no filter". Passing null straight
            // into Contains would throw instead.
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


        //part C
        public async Task<IActionResult> Top(int count)
        {
            if (!(0<count&& count<5))
            {
                return NotFound();
            }
            var students = await _context.Students
                .OrderByDescending(s => s.Gpa)
                .Take(count)
                .ToListAsync();

            return View("Index", students);
        }



        //Part D
        public async Task<IActionResult> Intake(string code)
        {

            if (string.IsNullOrWhiteSpace(code))
            {
                return NotFound();
            }

            if (!string.Equals(code, "itiA", StringComparison.OrdinalIgnoreCase))
            {
                return NotFound();
            }
            var students = await _context.Students.ToListAsync();
            return View("Index", students);
        }

        //Part E    
        [Route("about/muhamed")] 
        public async Task<IActionResult> About([FromQuery] double minGpa = 2.5)
        {
            var students = await _context.Students
                .Where(s => s.Gpa >= minGpa)
                .OrderBy(s => s.FullName    )
                .ToListAsync();

            return View("Index", students);
        }
    }
}
