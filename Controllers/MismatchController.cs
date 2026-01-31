using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class MismatchController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MismatchController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var mismatches = await _context.Mismatches
                .Include(m => m.Barang)
                .Include(m => m.Plant)
                .Include(m => m.Area)
                .OrderByDescending(m => m.WaktuKejadian)
                .ToListAsync();

            return View(mismatches);
        }
    }
}