
using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Repositories
{
    public interface IUsuarioRepository
    {
        Usuario GetByEmail(string email);
        Usuario GetById(int id);
        int GetCurrentUserId();
        void UpdatePassword(int id, string password);
        void UpdateProfile(Usuario usuario);
        void UpdateAvatar(int id, string avatar);
        void RemoveAvatar(int id);
    }
}
