using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly PropietarioRepository _propietarioRepository;

        public PropietarioController(IConfiguration configuration)
        {
            _propietarioRepository = new PropietarioRepository(configuration);
        }

        public IActionResult Index()
        {
            var propietarios = _propietarioRepository.GetAll();
            return View(propietarios);
        }

        public IActionResult Details(int id)
        {
            var propietario = _propietarioRepository.GetById(id);
            return propietario == null ? NotFound() : View(propietario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            try
            {
                _propietarioRepository.Create(propietario);
                TempData["Success"] = "Propietario creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al crear el propietario.";
                return View(propietario);
            }
        }

        public IActionResult Edit(int id)
        {
            var propietario = _propietarioRepository.GetById(id);
            return propietario == null ? NotFound() : View(propietario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Propietario propietario)
        {
            try
            {
                _propietarioRepository.Update(id, propietario);
                TempData["Success"] = "Propietario actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el propietario.";
                return View(propietario);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            var propietario = _propietarioRepository.GetById(id);
            return propietario == null ? NotFound() : View(propietario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _propietarioRepository.Delete(id);
                TempData["Success"] = "Propietario eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el propietario.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
