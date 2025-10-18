using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly ContratoRepository _contratoRepository;
        private readonly UsuarioRepository _usuarioRepository;

        public ContratoController(IConfiguration configuration)
        {
            _contratoRepository = new ContratoRepository(configuration);
            _usuarioRepository = new UsuarioRepository(configuration);
        }

        public IActionResult Index()
        {
            var contratos = _contratoRepository.GetAll();
            return View(contratos);
        }

        public IActionResult Details(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            return contrato == null ? NotFound() : View(contrato);
        }

        public IActionResult Create()
        {
            ViewData["InquilinoId"] = new SelectList(_contratoRepository.GetInquilinos(), "Id", "NombreCompleto");
            ViewData["InmuebleId"] = new SelectList(_contratoRepository.GetInmuebles(), "Id", "Direccion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            if (_contratoRepository.VerificarSuperposicion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
            {
                ModelState.AddModelError("", "Las fechas de este contrato se superponen con un contrato existente para el mismo inmueble.");
                ViewData["InquilinoId"] = new SelectList(_contratoRepository.GetInquilinos(), "Id", "NombreCompleto", contrato.InquilinoId);
                ViewData["InmuebleId"] = new SelectList(_contratoRepository.GetInmuebles(), "Id", "Direccion", contrato.InmuebleId);
                return View(contrato);
            }

            try
            {
                var userEmail = User.Identity?.Name;
                if (string.IsNullOrEmpty(userEmail))
                {
                    TempData["Error"] = "No se pudo obtener la información del usuario.";
                    return View(contrato);
                }
                contrato.UsuarioIdCreador = _usuarioRepository.GetCurrentUserId(userEmail);
                _contratoRepository.Create(contrato);
                TempData["Success"] = "Contrato creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al crear el contrato.";
                return View(contrato);
            }
        }

        public IActionResult Edit(int id)
        {
            var contrato = _contratoRepository.GetById(id);
            return contrato == null ? NotFound() : View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
            if (_contratoRepository.VerificarSuperposicion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, id))
            {
                ModelState.AddModelError("", "Las fechas se superponen con un contrato existente para el mismo inmueble.");
                return View(contrato);
            }

            try
            {
                _contratoRepository.Update(id, contrato);
                TempData["Success"] = "Contrato actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el contrato.";
                return View(contrato);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            if (!User.IsInRole("Administrador"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }
            var contrato = _contratoRepository.GetById(id);
            return contrato == null ? NotFound() : View(contrato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                _contratoRepository.Delete(id);
                TempData["Success"] = "Contrato y sus pagos asociados han sido eliminados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el contrato.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        public IActionResult Terminar(int id)
        {
            return View("Terminar", new Contrato());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id, decimal Multa)
        {
            var userEmail = User.Identity?.Name;
            if (string.IsNullOrEmpty(userEmail))
            {
                TempData["Error"] = "No se pudo obtener la información del usuario.";
                return RedirectToAction(nameof(Index));
            }
            var usuarioId = _usuarioRepository.GetCurrentUserId(userEmail);
            _contratoRepository.TerminarContrato(id, Multa, usuarioId);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Renovar(int id)
        {
            var nuevoContrato = new Contrato();
            return View("Create", nuevoContrato);
        }
    }
}
