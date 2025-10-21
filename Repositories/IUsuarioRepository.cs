using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario GetByEmail(string email);
        // Aquí definiremos los demás métodos que necesitemos
    }
}
