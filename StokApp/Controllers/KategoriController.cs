using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class KategoriController : Controller
    {
        private readonly ApplicationDbContext _context;

        public KategoriController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            var kategoris = await _context.Kategoris.ToListAsync();
            return View(kategoris);
        }

        [HttpGet("Create")]
        [AuthorizeRole("Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(Kategori kategori)
        {
            if (ModelState.IsValid)
            {
                _context.Kategoris.Add(kategori);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        [HttpGet("Edit/{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var kategori = await _context.Kategoris.FindAsync(id);
            if (kategori == null) return NotFound();
            return View(kategori);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, Kategori kategori)
        {
            if (id != kategori.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(kategori);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Kategori berhasil diupdate!";
                return RedirectToAction(nameof(Index));
            }
            return View(kategori);
        }

        [HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var kategori = await _context.Kategoris.FindAsync(id);
            if (kategori == null) return NotFound();

            var adaBarang = await _context.Barangs.AnyAsync(b => b.KategoriId == id);
            if (adaBarang)
            {
                TempData["Error"] = "Kategori tidak bisa dihapus karena masih ada barang!";
                return RedirectToAction(nameof(Index));
            }

            _context.Kategoris.Remove(kategori);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Kategori berhasil dihapus!";
            return RedirectToAction(nameof(Index));
        }
    }
}