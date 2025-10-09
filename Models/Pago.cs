using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InmobiliariaWebApp.Models
{
    public class Pago
    {
        public int Id { get; set; }

        [Display(Name = "Número de Pago")]
        public int NumeroPago { get; set; }

        [Display(Name = "Contrato")]
        public int ContratoId { get; set; }

        [Display(Name = "Fecha de Pago")]
        public DateTime FechaPago { get; set; }

        public decimal Importe { get; set; }

        public string Detalle { get; set; } = "";

        public string Estado { get; set; } = "Vigente";

        [ForeignKey(nameof(ContratoId))]
        public Contrato? Contrato { get; set; }

        // Auditoría
        public int? UsuarioIdCreador { get; set; }
        [ForeignKey(nameof(UsuarioIdCreador))]
        public Usuario? Creador { get; set; }

        public int? UsuarioIdAnulador { get; set; }
        [ForeignKey(nameof(UsuarioIdAnulador))]
        public Usuario? Anulador { get; set; }
    }
}
