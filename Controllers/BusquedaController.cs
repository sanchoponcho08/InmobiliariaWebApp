using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class BusquedaController : Controller
    {
        private readonly IInmuebleRepository _inmuebleRepository;
        private readonly IContratoRepository _contratoRepository;

        public BusquedaController(IInmuebleRepository inmuebleRepository, IContratoRepository contratoRepository)
        {
            _inmuebleRepository = inmuebleRepository;
            _contratoRepository = contratoRepository;
        }

        public IActionResult Index()
        {
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre");
            return View();
        }

        [HttpPost]
        public IActionResult Buscar(DateTime fechaInicio, DateTime fechaFin, int tipoInmuebleId, decimal precioMin, decimal precioMax)
        {
            // inmuebles ocupados en esas fechas
            var todosLosContratos = _contratoRepository.GetAll();
            var inmueblesOcupadosIds = todosLosContratos
                .Where(c => (fechaInicio < c.FechaFin && fechaFin > c.FechaInicio))
                .Select(c => c.InmuebleId)
                .Distinct()
                .ToList();

            var todosLosInmuebles = _inmuebleRepository.GetAll();
            var consulta = todosLosInmuebles
                .Where(i => i.Disponible); // Solo inmuebles disponibles

            // Filtro No MOSTRAR ocupados
            if (inmueblesOcupadosIds.Any())
            {
                consulta = consulta.Where(i => !inmueblesOcupadosIds.Contains(i.Id));
            }

            // Filtramos x tipoInmuble
            if (tipoInmuebleId > 0)
            {
                consulta = consulta.Where(i => i.TipoInmuebleId == tipoInmuebleId);
            }

            // Filtro x precio
            if (precioMin > 0)
            {
                consulta = consulta.Where(i => i.Precio >= precioMin);
            }
            if (precioMax > 0 && precioMax > precioMin)
            {
                consulta = consulta.Where(i => i.Precio <= precioMax);
            }

            var inmueblesDisponibles = consulta.ToList();
            
            ViewData["TipoInmuebleId"] = new SelectList(_inmuebleRepository.GetTiposInmueble(), "Id", "Nombre", tipoInmuebleId);

            return View("Index", inmueblesDisponibles);
        }
    }
}
