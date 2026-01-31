using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class BarangKeluarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangKeluarController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate, int? plantId, int? areaId)
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.Include(a => a.Plant).ToListAsync();

            var query = _context.BarangKeluars
                .Include(bk => bk.Barang)
                .Include(bk => bk.Plant)
                .Include(bk => bk.Area)
                .AsQueryable();

            if (startDate.HasValue)
                query = query.Where(bk => bk.Tanggal >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(bk => bk.Tanggal <= endDate.Value);

            if (plantId.HasValue)
                query = query.Where(bk => bk.PlantId == plantId.Value);

            if (areaId.HasValue)
                query = query.Where(bk => bk.AreaId == areaId.Value);

            var barangKeluars = await query.OrderByDescending(bk => bk.Tanggal).ToListAsync();

            return View(barangKeluars);
        }

        [HttpGet("Create")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).Include(b => b.StokBarang).ToListAsync();
            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.Include(a => a.Plant).ToListAsync();
            return View();
        }

        [HttpPost("Create")]
[ValidateAntiForgeryToken]
[AuthorizeRole("Admin")]
public async Task<IActionResult> Create(BarangKeluar barangKeluar)
{
    try
    {
        // ⭐ PENTING! HAPUS validation error untuk navigation properties ⭐
        ModelState.Remove("Barang");
        ModelState.Remove("Plant");
        ModelState.Remove("Area");
        
        // Log untuk debugging
        Console.WriteLine($"BarangId: {barangKeluar.BarangId}");
        Console.WriteLine($"TujuanPenggunaan: {barangKeluar.TujuanPenggunaan}");
        Console.WriteLine($"PlantId: {barangKeluar.PlantId}");
        Console.WriteLine($"AreaId: {barangKeluar.AreaId}");
        Console.WriteLine($"PenanggungJawab: {barangKeluar.PenanggungJawab}");
        Console.WriteLine($"Jumlah: {barangKeluar.Jumlah}");
        
        if (ModelState.IsValid)
        {
            // Cek stok
            var stokBarang = await _context.StokBarangs
                .FirstOrDefaultAsync(s => s.BarangId == barangKeluar.BarangId);

            if (stokBarang == null || stokBarang.TotalStok < barangKeluar.Jumlah)
            {
                TempData["Error"] = $"Stok tidak mencukupi! Stok tersedia: {stokBarang?.TotalStok ?? 0}";
                ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).Include(b => b.StokBarang).ToListAsync();
                ViewBag.Plants = await _context.Plants.ToListAsync();
                ViewBag.Areas = await _context.Areas.Include(a => a.Plant).ToListAsync();
                return View(barangKeluar);
            }

            // Simpan barang keluar
            _context.BarangKeluars.Add(barangKeluar);
            await _context.SaveChangesAsync();

            // Update stok
            stokBarang.TotalStok -= barangKeluar.Jumlah;
            stokBarang.LastUpdated = DateTime.Now;
            await _context.SaveChangesAsync();
            
            Console.WriteLine($"Stok updated! Barang: {barangKeluar.BarangId}, New Total: {stokBarang.TotalStok}");

            // Cek mismatch - ambil barang masuk terakhir untuk barang ini
            var barangMasuk = await _context.BarangMasuks
                .Where(bm => bm.BarangId == barangKeluar.BarangId)
                .OrderByDescending(bm => bm.Tanggal)
                .FirstOrDefaultAsync();

            if (barangMasuk != null && barangMasuk.TujuanAwal != barangKeluar.TujuanPenggunaan)
            {
                var mismatch = new Mismatch
                {
                    BarangId = barangKeluar.BarangId,
                    TujuanAwal = barangMasuk.TujuanAwal,
                    TujuanAktual = barangKeluar.TujuanPenggunaan,
                    PlantId = barangKeluar.PlantId,
                    AreaId = barangKeluar.AreaId,
                    Jumlah = barangKeluar.Jumlah,
                    WaktuKejadian = DateTime.Now,
                    Catatan = $"Mismatch terdeteksi pada transaksi keluar ID: {barangKeluar.Id}"
                };
                _context.Mismatches.Add(mismatch);
                await _context.SaveChangesAsync();
                
                Console.WriteLine($"MISMATCH DETECTED! {barangMasuk.TujuanAwal} -> {barangKeluar.TujuanPenggunaan}");
            }

            TempData["Success"] = "Barang keluar berhasil dicatat dan stok telah diupdate!";
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

        ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).Include(b => b.StokBarang).ToListAsync();
        ViewBag.Plants = await _context.Plants.ToListAsync();
        ViewBag.Areas = await _context.Areas.Include(a => a.Plant).ToListAsync();
        return View(barangKeluar);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        TempData["Error"] = $"Gagal menyimpan: {ex.Message}";
        ViewBag.Barangs = await _context.Barangs.Include(b => b.Kategori).Include(b => b.StokBarang).ToListAsync();
        ViewBag.Plants = await _context.Plants.ToListAsync();
        ViewBag.Areas = await _context.Areas.Include(a => a.Plant).ToListAsync();
        return View(barangKeluar);
    }
}
    }
}