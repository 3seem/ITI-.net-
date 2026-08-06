using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;

namespace StudentPortalWeb.Pages.Roster
{
    public class DetailsModel : PageModel
    {
        private readonly StudentPortalContext _context;

        public DetailsModel(StudentPortalContext context)
        {
            _context = context;
        }

        public Student? Student { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Student = await _context.Students
                .Include(s => s.Enrollments)
                     .ThenInclude(e => e.Course)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (Student == null)
            {
               return NotFound();
            }

            return Page();

        }
    }
}
