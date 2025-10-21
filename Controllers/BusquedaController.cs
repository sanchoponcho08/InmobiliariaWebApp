using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class BusquedaController : Controller
    {
        // private readonly ApplicationDbContext _context;

        // public BusquedaController(ApplicationDbContext context)
        // {
        //     _context = context;
        // }

        public IActionResult Index()
        {
            // ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult Buscar(DateTime fechaInicio, DateTime fechaFin, int tipoInmuebleId, decimal precioMin, decimal precioMax)
        {
            // // inmuebles ocupados en esas fechas
            // var inmueblesOcupadosIds = _context.Contratos
            //     .Where(c => (fechaInicio < c.FechaFin && fechaFin > c.FechaInicio))
            //     .Select(c => c.InmuebleId)
            //     .Distinct()
            //     .ToList();

            // var consulta = _context.Inmuebles
            //     .Include(i => i.Dueño)
            //     .Include(i => i.Tipo)
            //     .Where(i => i.Disponible); // Solo inmuebles disponibles

            // // Filtro No MOSTRAR ocupados
            // if (inmueblesOcupadosIds.Any())
            // {
            //     consulta = consulta.Where(i => !inmueblesOcupadosIds.Contains(i.Id));
            // }

            // // Filtramos x tipoInmuble
            // if (tipoInmuebleId > 0)
            // {
            //     consulta = consulta.Where(i => i.TipoInmuebleId == tipoInmuebleId);
            // }

            // // Filtro x precio
            // if (precioMin > 0)
            // {
            //     consulta = consulta.Where(i => i.Precio >= precioMin);
            // }
            // if (precioMax > 0 && precioMax > precioMin)
            // {
            //     consulta = consulta.Where(i => i.Precio <= precioMax);
            // }

            // var inmueblesDisponibles = consulta.ToList();
            
            // ViewData["TipoInmuebleId"] = new SelectList(_context.TiposInmuebles, "Id", "Nombre", tipoInmuebleId);

            return View("Index", new List<Inmueble>());
        }
    }
}
