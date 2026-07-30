using Lab15.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Lab15_StudentPortalWeb.Services;
namespace Lab15.Controllers
{
    public class HomeController : Controller
    {
        //Part C

        private readonly StudentPortalContext _context;

        //Part E
        private readonly IMuhamadStampService _stampA;
        private readonly IMuhamadStampService _stampB;
        //public HomeController(StudentPortalContext context) // Constructor Injection
        //{
        //    _context = context;
        //}
        public HomeController(
            StudentPortalContext context,
            IMuhamadStampService stampA,
            IMuhamadStampService stampB) 
        {
            _context = context;
            _stampA = stampA;
            _stampB = stampB;
        }
        public async Task<IActionResult> Index() {


            //Part C
            //var students = await _context.Students
            //        .OrderBy(s => s.FullName)
            //        .ToListAsync();

            //return View(students);

            //Part E
            var all = (Owner: _stampA.Owner, StampA: _stampA.Stamp, StampB: _stampB.Stamp);

            return View(all);
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
