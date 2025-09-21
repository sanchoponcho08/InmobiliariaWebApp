using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContratoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var contratos = _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ThenInclude(i => i.Dueño); // Incluimos al dueño a través del inmueble
            return View(await contratos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ThenInclude(i => i.Dueño)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        public IActionResult Create()
        {
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto");
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InquilinoId,InmuebleId,FechaInicio,FechaFin,MontoAlquiler")] Contrato contrato)
        {
            ModelState.Remove("Inquilino");
            ModelState.Remove("Inmueble");
            if (ModelState.IsValid)
            {
                _context.Add(contrato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InquilinoId,InmuebleId,FechaInicio,FechaFin,MontoAlquiler")] Contrato contrato)
        {
            if (id != contrato.Id) return NotFound();
            ModelState.Remove("Inquilino");
            ModelState.Remove("Inmueble");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contrato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Contratos.Any(e => e.Id == contrato.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato != null)
            {
                _context.Contratos.Remove(contrato);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}