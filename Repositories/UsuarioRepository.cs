using System.Data;
using System.Security.Claims;
using InmobiliariaWebApp.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;

namespace InmobiliariaWebApp.Repositories
{
    public class UsuarioRepository : RepositoryBase, IUsuarioRepository
    {
        public UsuarioRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public Usuario GetByEmail(string email)
        {
            Usuario usuario = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT Id, Nombre, Apellido, Email, Clave, Rol, AvatarUrl FROM Usuarios WHERE Email = @Email";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32(nameof(Usuario.Id)),
                                Nombre = reader.GetString(nameof(Usuario.Nombre)),
                                Apellido = reader.GetString(nameof(Usuario.Apellido)),
                                Email = reader.GetString(nameof(Usuario.Email)),
                                Clave = reader.GetString(nameof(Usuario.Clave)),
                                Rol = reader.GetInt32(nameof(Usuario.Rol)),
                                AvatarUrl = reader.IsDBNull(reader.GetOrdinal("AvatarUrl")) ? null : reader.GetString(nameof(Usuario.AvatarUrl)),
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        // Aquí irían los demás métodos CRUD (GetAll, GetById, Save, Delete) implementados de forma similar
        // Por ahora, nos centramos en el login para probar que la nueva estructura funciona.
    }
}
