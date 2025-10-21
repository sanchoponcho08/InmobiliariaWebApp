using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Repositories
{
    public interface IInmuebleRepository
    {
        List<Inmueble> GetAll();
        Inmueble? GetById(int id);
        void Create(Inmueble inmueble);
        void Update(Inmueble inmueble);
        void Delete(int id);
        bool Exists(int id);
        List<Propietario> GetPropietarios();
        List<TipoInmueble> GetTiposInmueble();
    }
}
