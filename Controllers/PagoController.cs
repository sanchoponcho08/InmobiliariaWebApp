using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly PagoRepository _pagoRepository;
        private readonly UsuarioRepository _usuarioRepository;
        private readonly ContratoRepository _contratoRepository;

        public PagoController(IConfiguration configuration)
        {
            _pagoRepository = new PagoRepository(configuration);
            _usuarioRepository = new UsuarioRepository(configuration);
            _contratoRepository = new ContratoRepository(configuration);
        }

        public IActionResult Index(int id)
        {
            ViewBag.ContratoId = id;
            var pagos = _pagoRepository.GetPagosByContratoId(id);
            var contrato = _contratoRepository.GetById(id);
            ViewBag.ContratoInfo = $"Contrato de {contrato.Inquilino.Nombre} {contrato.Inquilino.Apellido} sobre {contrato.Inmueble.Direccion}";

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
                pago.UsuarioIdCreador = _usuarioRepository.GetCurrentUserId(User.Identity.Name);
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
                var usuarioId = _usuarioRepository.GetCurrentUserId(User.Identity.Name);
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
