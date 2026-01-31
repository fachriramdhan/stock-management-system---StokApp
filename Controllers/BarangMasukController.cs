using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class BarangMasukController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangMasukController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? plantId, int? barangId)
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Barangs = await _context.Barangs.ToListAsync();

            var query = _context.BarangMasuks
                .Include(bm => bm.Barang)
                .Include(bm => bm.Plant)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(bm => bm.Tanggal >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(bm => bm.Tanggal <= endDate.Value);

            if (plantId.HasValue)
                query = query.Where(bm => bm.PlantId == plantId.Value);

            if (barangId.HasValue)
                query = query.Where(bm => bm.BarangId == barangId.Value);

            var barangMasuks = await query.OrderByDescending(bm => bm.Tanggal).ToListAsync();

            return View(barangMasuks);
        }

        [HttpGet("Create")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).ToListAsync();
            ViewBag.Plants = await _context.Plants.ToListAsync();
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(BarangMasuk barangMasuk)
        {
            try
            {
                // ⭐ PENTING! HAPUS validation error untuk navigation properties ⭐
                ModelState.Remove("Barang");
                ModelState.Remove("Plant");
                
                // Log untuk debugging
                Console.WriteLine($"BarangId: {barangMasuk.BarangId}");
                Console.WriteLine($"AsalBarang: {barangMasuk.AsalBarang}");
                Console.WriteLine($"TujuanAwal: {barangMasuk.TujuanAwal}");
                Console.WriteLine($"PlantId: {barangMasuk.PlantId}");
                Console.WriteLine($"LokasiPenyimpanan: {barangMasuk.LokasiPenyimpanan}");
                Console.WriteLine($"Jumlah: {barangMasuk.Jumlah}");
                Console.WriteLine($"Tanggal: {barangMasuk.Tanggal}");
                
                if (ModelState.IsValid)
                {
                    // Simpan barang masuk
                    _context.BarangMasuks.Add(barangMasuk);
                    await _context.SaveChangesAsync();

                    // Update stok
                    var stokBarang = await _context.StokBarangs
                        .FirstOrDefaultAsync(s => s.BarangId == barangMasuk.BarangId);

                    if (stokBarang != null)
                    {
                        stokBarang.TotalStok += barangMasuk.Jumlah;
                        stokBarang.LastUpdated = DateTime.Now;
                        await _context.SaveChangesAsync();
                        
                        Console.WriteLine($"Stok updated! Barang: {barangMasuk.BarangId}, New Total: {stokBarang.TotalStok}");
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: StokBarang not found for BarangId: {barangMasuk.BarangId}");
                    }

                    TempData["Success"] = "Barang masuk berhasil dicatat dan stok telah diupdate!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log validation errors
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).ToListAsync();
                ViewBag.Plants = await _context.Plants.ToListAsync();
                return View(barangMasuk);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Gagal menyimpan: {ex.Message}";
                ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).ToListAsync();
                ViewBag.Plants = await _context.Plants.ToListAsync();
                return View(barangMasuk);
            }
        }
    }
}