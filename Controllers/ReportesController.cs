
using InmobiliariaWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> ContratosVigentes()
        {
            var fechaActual = DateTime.Now;
            var contratos = await _context.Contratos
                .Include(c => c.Inquilino)
                .Include(c => c.Inmueble)
                .ThenInclude(i => i.Dueño)
                .Where(c => c.FechaInicio <= fechaActual && c.FechaFin >= fechaActual)
                .ToListAsync();

            return View(contratos);
        }
    }
}