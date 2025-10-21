using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Repositories
{
    public interface IPagoRepository
    {
        List<Pago> GetPagosByContratoId(int contratoId);
        Pago? GetById(int id);
        void Create(Pago pago);
        void Anular(int id, int usuarioId);
        int GetContratoIdByPagoId(int pagoId);
    }
}
