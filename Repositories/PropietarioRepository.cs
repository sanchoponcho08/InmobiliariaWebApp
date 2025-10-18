
using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Repositories
{
    public class PropietarioRepository
    {
        private readonly Conexion _conexion;

        public PropietarioRepository(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public List<Propietario> GetAll()
        {
            var propietarios = new List<Propietario>();
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Dni, Nombre, Apellido, Email, Telefono FROM Propietarios";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono")
                            });
                        }
                    }
                }
            }
            return propietarios;
        }

        public Propietario? GetById(int id)
        {
            Propietario? propietario = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Dni, Nombre, Apellido, Email, Telefono FROM Propietarios WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            propietario = new Propietario
                            {
                                Id = reader.GetInt32("Id"),
                                Dni = reader.GetString("Dni"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Telefono = reader.GetString("Telefono")
                            };
                        }
                    }
                }
            }
            return propietario;
        }

        public void Create(Propietario propietario)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "INSERT INTO Propietarios (Dni, Nombre, Apellido, Email, Telefono) VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono)";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Dni", propietario.Dni);
                    command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@Email", propietario.Email);
                    command.Parameters.AddWithValue("@Telefono", propietario.Telefono);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(int id, Propietario propietario)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Propietarios SET Dni = @Dni, Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Telefono = @Telefono WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Dni", propietario.Dni);
                    command.Parameters.AddWithValue("@Nombre", propietario.Nombre);
                    command.Parameters.AddWithValue("@Apellido", propietario.Apellido);
                    command.Parameters.AddWithValue("@Email", propietario.Email);
                    command.Parameters.AddWithValue("@Telefono", propietario.Telefono);
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "DELETE FROM Propietarios WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
