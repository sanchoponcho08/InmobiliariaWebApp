using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using InmobiliariaWebApp.Repositories;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly IInmuebleRepository _inmuebleRepository;

        public InmuebleController(IInmuebleRepository inmuebleRepository)
        {
            _inmuebleRepository = inmuebleRepository;
        }

        // GET: Inmueble
        public IActionResult Index()
        {
            var inmuebles = _inmuebleRepository.GetAll();
            return View(inmuebles);
        }

        // GET: Inmueble/Details/5
        public IActionResult Details(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            return View(inmueble);
        }

        // GET: Inmueble/Create
        public IActionResult Create()
        {
            ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto");
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre");
            return View();
        }

        // POST: Inmueble/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            if (ModelState.IsValid)
            {
                _inmuebleRepository.Create(inmueble);
                TempData["Success"] = "Inmueble creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // GET: Inmueble/Edit/5
        public IActionResult Edit(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
            if (inmueble == null)
            {
                return NotFound();
            }
            ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // POST: Inmueble/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            if (id != inmueble.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _inmuebleRepository.Update(inmueble);
                    TempData["Success"] = "Inmueble actualizado exitosamente.";
                }
                catch (System.Exception)
                {
                    if (!_inmuebleRepository.Exists(inmueble.Id))
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
            ViewData["PropietarioId"] = new SelectList(_inmuebleRepository.GetPropietarios(), "Id", "NombreCompleto", inmueble.PropietarioId);
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", inmueble.TipoInmuebleId);
            return View(inmueble);
        }

        // GET: Inmueble/Delete/5
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inmueble = _inmuebleRepository.GetById(id);
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
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _inmuebleRepository.Delete(id);
                TempData["Success"] = "Inmueble eliminado exitosamente.";
                 return RedirectToAction(nameof(Index));
            }
            catch (System.Exception)
            {
                TempData["Error"] = "Ocurrió un error al eliminar el inmueble. Es posible que esté asociado a un contrato.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }
    }
}
