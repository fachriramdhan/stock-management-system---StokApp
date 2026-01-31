using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.Username = SessionHelper.GetSession(HttpContext, "Username");
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");

            // Total statistik
            ViewBag.TotalBarang = await _context.Barangs.CountAsync();
            ViewBag.TotalStok = await _context.StokBarangs.SumAsync(s => s.TotalStok);
            ViewBag.TotalTools = await _context.Tools.CountAsync();
            ViewBag.TotalPlant = await _context.Plants.CountAsync();
            ViewBag.TotalArea = await _context.Areas.CountAsync();

            // Stok kritis
            var stokKritis = await _context.StokBarangs
                .Include(s => s.Barang)
                .Where(s => s.TotalStok <= s.Barang.MinimumStok)
                .OrderBy(s => s.TotalStok)
                .Take(10)
                .ToListAsync();
            ViewBag.StokKritis = stokKritis;

            // Total mismatch
            ViewBag.TotalMismatch = await _context.Mismatches.CountAsync();

            // Kebutuhan replace tanpa stok
            var replaceData = await _context.BarangKeluars
                .Include(bk => bk.Barang)
                    .ThenInclude(b => b.StokBarang)
                .Include(bk => bk.Plant)
                .Include(bk => bk.Area)
                .Where(bk => bk.TujuanPenggunaan == "Replace" && 
                            bk.Barang.StokBarang!.TotalStok == 0)
                .OrderByDescending(bk => bk.Tanggal)
                .Take(5)
                .ToListAsync();
            ViewBag.ReplaceNoStok = replaceData;

            // Recent activity
            var recentMasuk = await _context.BarangMasuks
                .Include(bm => bm.Barang)
                .Include(bm => bm.Plant)
                .OrderByDescending(bm => bm.Tanggal)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentMasuk = recentMasuk;

            var recentKeluar = await _context.BarangKeluars
                .Include(bk => bk.Barang)
                .Include(bk => bk.Plant)
                .Include(bk => bk.Area)
                .OrderByDescending(bk => bk.Tanggal)
                .Take(5)
                .ToListAsync();
            ViewBag.RecentKeluar = recentKeluar;

            return View();
        }
    }
}