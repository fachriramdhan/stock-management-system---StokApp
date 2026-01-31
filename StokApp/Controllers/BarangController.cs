using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class BarangController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BarangController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Barang atau /Barang/Index
        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            
            var barangs = await _context.Barangs
                .Include(b => b.Kategori)
                .Include(b => b.StokBarang)
                .OrderBy(b => b.Nama)
                .ToListAsync();

            return View(barangs);
        }

        // GET: /Barang/Create
        [HttpGet("Create")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Kategoris = await _context.Kategoris.ToListAsync();
            return View();
        }

    // POST: /Barang/Create
[HttpPost("Create")]
[ValidateAntiForgeryToken]
[AuthorizeRole("Admin")]
public async Task<IActionResult> Create(Barang barang)
{
    try
    {
        // HAPUS validation error untuk Kategori navigation property
        ModelState.Remove("Kategori");
        
        // Log untuk debugging
        Console.WriteLine($"Nama: {barang.Nama}");
        Console.WriteLine($"KategoriId: {barang.KategoriId}");
        Console.WriteLine($"Satuan: {barang.Satuan}");
        Console.WriteLine($"MinimumStok: {barang.MinimumStok}");
        
        if (ModelState.IsValid)
        {
            _context.Barangs.Add(barang);
            await _context.SaveChangesAsync();

            // Buat stok barang otomatis
            var stokBarang = new StokBarang
            {
                BarangId = barang.Id,
                TotalStok = 0,
                LastUpdated = DateTime.Now
            };
            _context.StokBarangs.Add(stokBarang);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Barang berhasil ditambahkan!";
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

        ViewBag.Kategoris = await _context.Kategoris.ToListAsync();
        return View(barang);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine($"StackTrace: {ex.StackTrace}");
        TempData["Error"] = $"Gagal menyimpan: {ex.Message}";
        ViewBag.Kategoris = await _context.Kategoris.ToListAsync();
        return View(barang);
    }
}

        // GET: /Barang/Edit/5
        [HttpGet("Edit/{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var barang = await _context.Barangs.FindAsync(id);
            if (barang == null) return NotFound();

            ViewBag.Kategoris = await _context.Kategoris.ToListAsync();
            return View(barang);
        }

        // POST: /Barang/Edit/5
        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, Barang barang)
        {
            if (id != barang.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(barang);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Barang berhasil diupdate!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BarangExists(barang.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Kategoris = await _context.Kategoris.ToListAsync();
            return View(barang);
        }

        // POST: /Barang/Delete/5
        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var barang = await _context.Barangs.FindAsync(id);
            if (barang == null) return NotFound();

            // Cek apakah barang sudah pernah ada transaksi
            var adaTransaksi = await _context.BarangMasuks.AnyAsync(bm => bm.BarangId == id) ||
                              await _context.BarangKeluars.AnyAsync(bk => bk.BarangId == id);

            if (adaTransaksi)
            {
                TempData["Error"] = "Barang tidak bisa dihapus karena sudah ada transaksi!";
                return RedirectToAction(nameof(Index));
            }

            _context.Barangs.Remove(barang);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Barang berhasil dihapus!";

            return RedirectToAction(nameof(Index));
        }

        private bool BarangExists(int id)
        {
            return _context.Barangs.Any(e => e.Id == id);
        }
    }
}