using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentPortalWeb.Models;

namespace StudentPortalWeb.Pages.Roster
{
    public class IndexModel : PageModel
    {
        private readonly StudentPortalContext _context;

        public IndexModel(StudentPortalContext context)
        {
            _context = context;
        }

        public List<Student> Students { get; set; } = new();

        public async Task OnGetAsync()
        {
            Students = await _context.Students
                .OrderBy(s => s.FullName)
                .ToListAsync();
        }
    }
}
