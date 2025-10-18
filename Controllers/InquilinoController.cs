using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly InquilinoRepository _inquilinoRepository;

        public InquilinoController(IConfiguration configuration)
        {
            _inquilinoRepository = new InquilinoRepository(configuration);
        }

        public IActionResult Index()
        {
            var inquilinos = _inquilinoRepository.GetAll();
            return View(inquilinos);
        }

        public IActionResult Details(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            return inquilino == null ? NotFound() : View(inquilino);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            try
            {
                _inquilinoRepository.Create(inquilino);
                TempData["Success"] = "Inquilino creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al crear el inquilino.";
                return View(inquilino);
            }
        }

        public IActionResult Edit(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            return inquilino == null ? NotFound() : View(inquilino);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            try
            {
                _inquilinoRepository.Update(id, inquilino);
                TempData["Success"] = "Inquilino actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el inquilino.";
                return View(inquilino);
                
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var inquilino = _inquilinoRepository.GetById(id);
            return inquilino == null ? NotFound() : View(inquilino);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _inquilinoRepository.Delete(id);
                TempData["Success"] = "Inquilino eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el inquilino.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }
    }
}
