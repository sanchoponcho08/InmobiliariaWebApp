using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaWebApp.Models
{
    public class Contrato
    {
        public int Id { get; set; }

        [Display(Name = "Inquilino")]
        public int InquilinoId { get; set; }

        [Display(Name = "Inmueble")]
        public int InmuebleId { get; set; }

        [Display(Name = "Fecha de Inicio")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de Finalización")]
        public DateTime FechaFin { get; set; }

        [Display(Name = "Monto del Alquiler")]
        public decimal MontoAlquiler { get; set; }
        
        [Display(Name = "Fecha de Rescisión")]
        public DateTime? FechaRescision { get; set; }

        public decimal Multa { get; set; }

        [ForeignKey(nameof(InquilinoId))]
        public Inquilino? Inquilino { get; set; }

        [ForeignKey(nameof(InmuebleId))]
        public Inmueble? Inmueble { get; set; }

        // Auditoría
        public int? UsuarioIdCreador { get; set; }
        [ForeignKey(nameof(UsuarioIdCreador))]
        public Usuario? Creador { get; set; }

        public int? UsuarioIdTerminador { get; set; }
        [ForeignKey(nameof(UsuarioIdTerminador))]
        public Usuario? Terminador { get; set; }
    }
}
