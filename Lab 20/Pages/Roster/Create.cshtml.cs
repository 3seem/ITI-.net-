// LAB 20 — Lab ID: 27 | MIN_GPA_LAB = 2.5 | MAX_YEAR_LAB = 2
//Part A

//(a)StudentsController's two Create methods can share a name because MVC choose by
//[HttpGet]/[HttpPost] attributes at the action selection stage the framework picks
//the right overload before it ever needs a unique method name A Razor Pages class has no action
//selector handlers are located by reflecting over method names for the [Async] pattern


//(B) Because Student is a property, it's visible to every handler on the page not just the
//one it was declared in and to the .cshtml view via Model.Student

//(C) Returning the view/page after a successful POST still leaves the browser sitting on a
// POST request in its histor Pressing F5 re-submits that exact request again



using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using StudentPortalWeb.Models;

namespace StudentPortalWeb.Pages.Roster
{
    //Part B
    public class CreateModel : PageModel
    {

        private readonly StudentPortalContext _context;

        public CreateModel(StudentPortalContext context)
        {
            _context = context;
        }
        //Part D 4 if  u want to see parts before uncomment the next line
        //[BindProperty]

        //Part D 4
        //Student never gets bound from the post it stays the default new () (null FullName, YearOfStudy = 0, Gpa = 0.0).
        //Since Student was never part of model binding, it's also never part of automatic validation


        public Student Student { get; set; } = new();

        public void OnGet()
        {
            // anew form needs no data loaded from the database
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            double MIN_GPA_LAB = 2.5;
            int MAX_YEAR_LAB = 2;

            if (Student.Gpa < MIN_GPA_LAB)
            {
                ModelState.AddModelError("Student.Gpa",
                    $"GPA must be at least {MIN_GPA_LAB} for this intake.");
            }

            if (Student.YearOfStudy > MAX_YEAR_LAB)
            {
                ModelState.AddModelError("Student.YearOfStudy",
                    $"Year of study may not exceed {MAX_YEAR_LAB} for this intake.");
            }
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _context.Students.AddAsync(Student);
            await _context.SaveChangesAsync();

            TempData["Message"] = $"{Student.FullName} was added the Razor Pages way";

            return RedirectToPage("./Index");
        }
    }
}
