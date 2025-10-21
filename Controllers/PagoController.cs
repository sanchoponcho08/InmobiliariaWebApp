using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly IPagoRepository _pagoRepository;
        private readonly IContratoRepository _contratoRepository;

        public PagoController(IPagoRepository pagoRepository, IContratoRepository contratoRepository)
        {
            _pagoRepository = pagoRepository;
            _contratoRepository = contratoRepository;
        }

        public IActionResult Index(int id)
        {
            ViewBag.ContratoId = id;
            var pagos = _pagoRepository.GetPagosByContratoId(id);
            var contrato = _contratoRepository.GetById(id);
            if (contrato != null && contrato.Inquilino != null && contrato.Inmueble != null)
            {
                ViewBag.ContratoInfo = $"Contrato de {contrato.Inquilino.Nombre} {contrato.Inquilino.Apellido} sobre {contrato.Inmueble.Direccion}";
            }
            else
            {
                ViewBag.ContratoInfo = "Información del contrato no disponible";
            }

            return View(pagos);
        }

        public IActionResult Create(int id)
        {
            var pago = new Pago { ContratoId = id };
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            try
            {
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdValue, out var userId))
                {
                    return Unauthorized();
                }
                pago.UsuarioIdCreador = userId;
                _pagoRepository.Create(pago);
                TempData["Success"] = "Pago registrado exitosamente.";
                return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al registrar el pago.";
                return View(pago);
            }
        }

        // GET: Pago/Edit/5
        public IActionResult Edit(int id)
        {
            var pago = _pagoRepository.GetById(id);
            if (pago == null)
            {
                return NotFound();
            }
            return View(pago);
        }

        // POST: Pago/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Pago pago)
        {
            if (id != pago.Id)
            {
                return BadRequest();
            }

            try
            {
                _pagoRepository.Update(pago);
                TempData["Success"] = "Pago actualizado correctamente.";
                return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el pago.";
                return View(pago);
            }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            if (!User.IsInRole("Administrador"))
            {
                return RedirectToAction("AccessDenied", "Home");
            }

            var pago = _pagoRepository.GetById(id);
            return pago == null ? NotFound() : View(pago);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            int contratoId = 0;
            try
            {
                contratoId = _pagoRepository.GetContratoIdByPagoId(id);
                var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (!int.TryParse(userIdValue, out var usuarioId))
                {
                    return Unauthorized();
                }
                _pagoRepository.Anular(id, usuarioId);
                TempData["Success"] = "Pago anulado correctamente.";
                return RedirectToAction(nameof(Index), new { id = contratoId });
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al anular el pago.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        public IActionResult Details(int id)
        {
            var pago = _pagoRepository.GetById(id);
            return pago == null ? NotFound() : View(pago);
        }
    }
}
