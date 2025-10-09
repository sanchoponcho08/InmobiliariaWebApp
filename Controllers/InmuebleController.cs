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

        // GET: Inmueble
        public async Task<IActionResult> Index()
        {
            var inmuebles = await _context.Inmuebles
                .Include(i => i.Dueño)
                .Include(i => i.Tipo)
                .ToListAsync();
            return View(inmuebles);
        }

        // GET: Inmueble/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles
                .Include(i => i.Dueño)
                .Include(i => i.Tipo)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto");
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre");
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Direccion,Uso,TipoInmuebleId,Ambientes,Precio,Coordenadas,Disponible,PropietarioId")] Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                _context.Add(inmueble);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Inmueble creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles.FindAsync(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Direccion,Uso,TipoInmuebleId,Ambientes,Precio,Coordenadas,Disponible,PropietarioId")] Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(inmueble);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inmueble actualizado exitosamente.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!InmuebleExists(inmueble.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropietarioId"] = new SelectList(_context.Propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var inmueble = await _context.Inmuebles
                .Include(i => i.Dueño)
                .Include(i => i.Tipo)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (inmueble == null)
            {
                return NotFound();
            }

            return View(inmueble);
        }

        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var inmueble = await _context.Inmuebles.FindAsync(id);
            try
            {
                if (inmueble != null)
                {
                    _context.Inmuebles.Remove(inmueble);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Inmueble eliminado exitosamente.";
                }
                 return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el inmueble. Es posible que esté asociado a un contrato.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

           
        }

        private bool InmuebleExists(int id)
        {
            return _context.Inmuebles.Any(e => e.Id == id);
        }
    }
}
