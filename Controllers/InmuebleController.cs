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
        // private readonly InmuebleRepository _inmuebleRepository;

        // public InmuebleController(ApplicationDbContext context)
        // {
        //     _inmuebleRepository = new InmuebleRepository(context);
        // }

        // GET: Inmueble
        public async Task<IActionResult> Index()
        {
            // var inmuebles = await _inmuebleRepository.GetAllAsync();
            return View(new List<Inmueble>());
        }

        // GET: Inmueble/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var inmueble = await _inmuebleRepository.GetByIdAsync(id);

            // if (inmueble == null)
            // {
            //     return NotFound();
            // }

            return View(new Inmueble());
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            // ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto");
            // ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre");
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Direccion,Uso,TipoInmuebleId,Ambientes,Precio,Coordenadas,Disponible,PropietarioId")] Inmueble inmueble)
        {
            // if (ModelState.IsValid)
            // {
            //     await _inmuebleRepository.CreateAsync(inmueble);
            //     TempData["Success"] = "Inmueble creado exitosamente.";
            //     return RedirectToAction(nameof(Index));
            // }
            // ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            // ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // var inmueble = await _inmuebleRepository.FindAsync(id);
            // if (inmueble == null)
            // {
            //     return NotFound();
            // }
            // ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            // ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(new Inmueble());
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

            // if (ModelState.IsValid)
            // {
            //     try
            //     {
            //         await _inmuebleRepository.UpdateAsync(inmueble);
            //         TempData["Success"] = "Inmueble actualizado exitosamente.";
            //     }
            //     catch (DbUpdateConcurrencyException)
            //     {
            //         if (!_inmuebleRepository.Exists(inmueble.Id))
            //         {
            //             return NotFound();
            //         }
            //         else
            //         {
            //             throw;
            //         }
            //     }
            //     return RedirectToAction(nameof(Index));
            // }
            // ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            // ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
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

            // var inmueble = await _inmuebleRepository.GetByIdAsync(id);
            // if (inmueble == null)
            // {
            //     return NotFound();
            // }

            return View(new Inmueble());
        }

        // POST: Inmueble/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // try
            // {
            //     await _inmuebleRepository.DeleteAsync(id);
            //     TempData["Success"] = "Inmueble eliminado exitosamente.";
            //      return RedirectToAction(nameof(Index));
            // }
            // catch (Exception)
            // {
            //     TempData["Error"] = "Ocurrió un error al eliminar el inmueble. Es posible que esté asociado a un contrato.";
            //     return RedirectToAction(nameof(Delete), new { id = id });
            // }

           return RedirectToAction(nameof(Index));
        }
    }
}
