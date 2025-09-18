using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InmuebleController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var inmuebles = _context.Inmuebles
                .Include(i => i.Dueño) //  datos del propietario
                .Include(i => i.Tipo);  //  tipo de inmueble
            return View(await inmuebles.ToListAsync());
        }

        public IActionResult Create()
        {
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto");
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Direccion,Uso,TipoInmuebleId,Ambientes,Precio,Coordenadas,Disponible,PropietarioId")] Inmueble inmueble)
        {

            ModelState.Remove("Tipo");
            ModelState.Remove("Dueño");

            if (ModelState.IsValid)
            {
                _context.Add(inmueble);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null) return NotFound();

            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Direccion,Uso,TipoInmuebleId,Ambientes,Precio,Coordenadas,Disponible,PropietarioId")] Inmueble inmueble)
        {
            if (id != inmueble.Id) return NotFound();

            ModelState.Remove("Tipo");
            ModelState.Remove("Dueño");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inmueble);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Inmuebles.Any(e => e.Id == inmueble.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var inmueble = await _context.Inmuebles
                .Include(i => i.Dueño)
                .Include(i => i.Tipo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inmueble == null) return NotFound();

            return View(inmueble);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble != null)
            {
                _context.Inmuebles.Remove(inmueble);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}

