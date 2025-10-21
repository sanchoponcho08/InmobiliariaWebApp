
using System.Data;
using System.Security.Claims;
using InmobiliariaWebApp.Models;
using Microsoft.Extensions.Configuration;
using MySqlConnector;

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
                string query = @"SELECT Id, Nombre, Apellido, Email, Clave, Rol, Avatar FROM Usuarios WHERE Email = @Email";
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
                                Rol = reader.GetString(nameof(Usuario.Rol)),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public Usuario GetById(int id)
        {
            Usuario usuario = null;
            using (MySqlConnection connection = new MySqlConnection(connectionString))
            {
                string query = @"SELECT Id, Nombre, Apellido, Email, Clave, Rol, Avatar FROM Usuarios WHERE Id = @Id";
                using (MySqlCommand command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
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
                                Rol = reader.GetString(nameof(Usuario.Rol)),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString(nameof(Usuario.Avatar)),
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public int GetCurrentUserId() { return 1;}
        public void UpdatePassword(int id, string password){}
        public void UpdateProfile(Usuario usuario){}
        public void UpdateAvatar(int id, string avatar){}
        public void RemoveAvatar(int id){}
    }
}
