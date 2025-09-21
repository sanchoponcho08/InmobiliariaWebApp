using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PagoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
        {
            ViewBag.ContratoId = id;
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(c => c.Id == id);
            
            if (contrato != null)
            {
                ViewBag.ContratoInfo = $"Contrato de {contrato.Inquilino.NombreCompleto} sobre {contrato.Inmueble.Direccion}";
            }

            var pagos = _context.Pagos
                .Include(p => p.Contrato)
                .Where(p => p.ContratoId == id);
                
            return View(await pagos.ToListAsync());
        }

        public IActionResult Create(int id)
        {
            ViewBag.ContratoId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Pago pago)
        {
            ModelState.Remove("Contrato");
            if (ModelState.IsValid)
            {
                _context.Add(pago);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
            }
            return View(pago);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var pago = await _context.Pagos.FindAsync(id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Pago pago)
        {
            if (id != pago.Id) return NotFound();

            ModelState.Remove("Contrato");
            if (ModelState.IsValid)
            {
                try
                {
                    //  Solo se puede editar el detalle
                    var pagoOriginal = await _context.Pagos.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
                    if(pagoOriginal != null)
                    {
                        pago.NumeroPago = pagoOriginal.NumeroPago;
                        pago.FechaPago = pagoOriginal.FechaPago;
                        pago.Importe = pagoOriginal.Importe;
                    }

                    _context.Update(pago);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pagos.Any(e => e.Id == pago.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
            }
            return View(pago);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var pago = await _context.Pagos
                .Include(p => p.Contrato)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pago == null) return NotFound();
            return View(pago);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pago = await _context.Pagos.FindAsync(id);
            if (pago != null)
            {
                //  cambiamos el estado en lugar de borrar
                pago.Estado = "Anulado";
                _context.Update(pago);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
        }
    }
}