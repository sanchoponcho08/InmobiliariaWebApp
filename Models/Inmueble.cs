using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaWebApp.Models
{
    public class Inmueble
    {
        public int Id { get; set; }

        public string Direccion { get; set; } = "";
        
        public string Uso { get; set; } = "";

        [Display(Name = "Tipo de Inmueble")]
        public int TipoInmuebleId { get; set; }
        
        [Display(Name = "Cantidad de Ambientes")]
        public int Ambientes { get; set; }

        public decimal Precio { get; set; }

        public string Coordenadas { get; set; } = "";
        
        // disponibilidad 
        public bool Disponible { get; set; }

        [Display(Name = "Dueño")]
        public int PropietarioId { get; set; }

        
        [ForeignKey(nameof(TipoInmuebleId))]
        public TipoInmueble? Tipo { get; set; }

        [ForeignKey(nameof(PropietarioId))]
        public Propietario? Dueño { get; set; }
    }
}