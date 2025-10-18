using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using MySql.Data.MySqlClient;

namespace InmobiliariaWebApp.Repositories
{
    public class UsuarioRepository
    {
        private readonly Conexion _conexion;

        public UsuarioRepository(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public int GetCurrentUserId(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                return 0;
            }

            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", userEmail);
                    connection.Open();
                    var id = command.ExecuteScalar();
                    return id != null ? Convert.ToInt32(id) : 0;
                }
            }
        }

        public Usuario? GetByEmail(string email)
        {
            Usuario? usuario = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Clave, Rol, Avatar FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Clave = reader.GetString("Clave"),
                                Rol = reader.GetString("Rol"),
                                Avatar = reader.IsDBNull(reader.GetOrdinal("Avatar")) ? null : reader.GetString("Avatar")
                            };
                        }
                    }
                }
            }
            return usuario;
        }

        public void UpdateProfile(string email, string nombre, string apellido)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Usuarios SET Nombre = @Nombre, Apellido = @Apellido WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Nombre", nombre);
                    command.Parameters.AddWithValue("@Apellido", apellido);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdatePassword(string email, string newPassword)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Usuarios SET Clave = @ClaveNueva WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@ClaveNueva", newPassword);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void UpdateAvatar(string email, string avatarUrl)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Usuarios SET Avatar = @Avatar WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Avatar", avatarUrl);
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void RemoveAvatar(string email)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Usuarios SET Avatar = NULL WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
