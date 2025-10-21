using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Repositories
{
    public interface IContratoRepository
    {
        List<Contrato> GetAll();
        Contrato GetById(int id);
        List<Inquilino> GetInquilinos();
        List<Inmueble> GetInmuebles();
        bool VerificarSuperposicion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null);
        void Create(Contrato contrato);
        void Update(int id, Contrato contrato);
        void Delete(int id);
        void TerminarContrato(int id, decimal multa, int usuarioId);
    }
}
