using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StokApp.Data;
using StokApp.Models;
using StokApp.Helpers;

namespace StokApp.Controllers
{
    [Route("[controller]")]
    [AuthorizeRole("Admin", "Supervisor")]
    public class ToolsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ToolsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("")]
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            ViewBag.UserRole = SessionHelper.GetSession(HttpContext, "UserRole");
            var tools = await _context.Tools
                .Include(t => t.Plant)
                .Include(t => t.Area)
                .OrderBy(t => t.Nama)
                .ToListAsync();

            return View(tools);
        }

        [HttpGet("Create")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.ToListAsync();
            return View();
        }

        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Create(Tools tools)
        {
            if (ModelState.IsValid)
            {
                _context.Tools.Add(tools);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tools berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.ToListAsync();
            return View(tools);
        }

        [HttpGet("Edit/{id}")]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var tools = await _context.Tools.FindAsync(id);
            if (tools == null) return NotFound();

            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.ToListAsync();
            return View(tools);
        }

        [HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        [AuthorizeRole("Admin")]
        public async Task<IActionResult> Edit(int id, Tools tools)
        {
            if (id != tools.Id) return NotFound();

            if (ModelState.IsValid)
            {
                _context.Update(tools);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Tools berhasil diupdate!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Plants = await _context.Plants.ToListAsync();
            ViewBag.Areas = await _context.Areas.ToListAsync();
            return View(tools);
        }
    }
}