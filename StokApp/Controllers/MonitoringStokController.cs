using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class MonitoringStokController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MonitoringStokController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(int? kategoriId)
        {
            ViewBag.Kategoris = await _context.Kategoris.ToListAsync();

            var query = _context.StokBarangs
                .Include(s => s.Barang)
                    .ThenInclude(b => b.Kategori)
                .AsQueryable();

            if (kategoriId.HasValue)
            {
                query = query.Where(s => s.Barang.KategoriId == kategoriId.Value);
            }

            var stokBarangs = await query
                .OrderBy(s => s.TotalStok)
                .ToListAsync();

            return View(stokBarangs);
        }
    }
}