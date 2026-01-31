using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class PlantController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PlantController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            var plants = await _context.Plants.Include(p => p.Areas).ToListAsync();
            return View(plants);
        }

        [HttpGet("CreatePlant")]
        [AuthorizeRole("Admin")]
        public IActionResult CreatePlant()
        {
            return View();
        }

        [HttpPost("CreatePlant")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreatePlant(Plant plant)
        {
            if (ModelState.IsValid)
            {
                _context.Plants.Add(plant);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Plant berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(plant);
        }

        [HttpGet("CreateArea")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateArea()
        {
            ViewBag.Plants = await _context.Plants.ToListAsync();
            return View();
        }

        [HttpPost("CreateArea")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> CreateArea(Area area)
        {
            try
            {
                // ⭐ INI YANG PENTING! ⭐
                // HAPUS validation error untuk Plant navigation property
                ModelState.Remove("Plant");
                
                // Log untuk debugging
                Console.WriteLine($"Nama Area: {area.Nama}");
                Console.WriteLine($"PlantId: {area.PlantId}");
                Console.WriteLine($"Deskripsi: {area.Deskripsi}");
                
                if (ModelState.IsValid)
                {
                    _context.Areas.Add(area);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Area berhasil ditambahkan!";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Log validation errors untuk debugging
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
                    }
                }

                ViewBag.Plants = await _context.Plants.ToListAsync();
                return View(area);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                Console.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = $"Gagal menyimpan: {ex.Message}";
                ViewBag.Plants = await _context.Plants.ToListAsync();
                return View(area);
            }
        }
    }
}