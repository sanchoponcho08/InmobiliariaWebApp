
using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    // Solo usuarios con el rol "Administrador" pueden acceder
    [Authorize(Roles = "Administrador")]
    public class TipoInmuebleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TipoInmuebleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.TiposInmuebles.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nombre")] TipoInmueble tipoInmueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tipoInmueble);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tipoInmueble = await _context.TiposInmuebles.FindAsync(id);
            if (tipoInmueble == null) return NotFound();
            return View(tipoInmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] TipoInmueble tipoInmueble)
        {
            if (id != tipoInmueble.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tipoInmueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TiposInmuebles.Any(e => e.Id == tipoInmueble.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tipoInmueble);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var tipoInmueble = await _context.TiposInmuebles.FirstOrDefaultAsync(m => m.Id == id);
            if (tipoInmueble == null) return NotFound();
            return View(tipoInmueble);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tipoInmueble = await _context.TiposInmuebles.FindAsync(id);
            if (tipoInmueble != null)
            {
                _context.TiposInmuebles.Remove(tipoInmueble);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}