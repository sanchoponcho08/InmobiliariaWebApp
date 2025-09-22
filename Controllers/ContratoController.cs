using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class ContratoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ContratoController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var contratos = _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ThenInclude(i => i.Dueño); // Incluimos al dueño a través del inmueble
            return View(await contratos.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ThenInclude(i => i.Dueño)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        public IActionResult Create()
        {
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto");
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InquilinoId,InmuebleId,FechaInicio,FechaFin,MontoAlquiler")] Contrato contrato)
        {
            ModelState.Remove("Inquilino");
            ModelState.Remove("Inmueble");
            if (ModelState.IsValid)
            {
                _context.Add(contrato);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InquilinoId,InmuebleId,FechaInicio,FechaFin,MontoAlquiler")] Contrato contrato)
        {
            if (id != contrato.Id) return NotFound();
            ModelState.Remove("Inquilino");
            ModelState.Remove("Inmueble");
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(contrato);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Contratos.Any(e => e.Id == contrato.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["InquilinoId"] = new SelectList(_context.Inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
            ViewData["InmuebleId"] = new SelectList(_context.Inmuebles, "Id", "Direccion", contrato.InmuebleId);
            return View(contrato);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var contrato = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contrato == null) return NotFound();
            return View(contrato);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato != null)
            {
                _context.Contratos.Remove(contrato);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Terminar(int? id)
        {
            if (id == null) return NotFound();

            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();
            var totalMeses = (contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12 + contrato.FechaFin.Month - contrato.FechaInicio.Month;
            var mesesCumplidos = (DateTime.Now.Year - contrato.FechaInicio.Year) * 12 + DateTime.Now.Month - contrato.FechaInicio.Month;

            if (mesesCumplidos < totalMeses / 2.0)
            {
                // Si cumplio menos de la mitad, la multa es de 2 meses
                ViewBag.Multa = contrato.MontoAlquiler * 2;
            }
            else
            {
                // Si cumplio más de la mitad, la multa es de 1 mes
                ViewBag.Multa = contrato.MontoAlquiler;
            }

            return View("Terminar", contrato);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Terminar(int id, decimal Multa)
        {
            var contrato = await _context.Contratos.FindAsync(id);
            if (contrato == null) return NotFound();

            // Actualizamos el contrato con la fecha de rescisión y la multa
            contrato.FechaRescision = DateTime.Now;
            contrato.Multa = Multa;
            _context.Update(contrato);

            // Creamos un nuevo pago para registrar la multa
            var pagoMulta = new Pago
            {
                ContratoId = contrato.Id,
                NumeroPago = 99, // Un número especial para identificar la multa
                FechaPago = DateTime.Now,
                Importe = Multa,
                Detalle = "Pago de multa por rescisión anticipada",
                Estado = "Vigente"
            };
            _context.Pagos.Add(pagoMulta);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}
