using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using MySqlConnector;
using System.Data;

namespace InmobiliariaWebApp.Repositories
{
    public class InquilinoRepository
    {
        private readonly Conexion _conexion;

        public InquilinoRepository(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public List<Inquilino> GetAll()
        {
            var inquilinos = new List<Inquilino>();
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Dni, Nombre, Apellido, Email, Telefono FROM Inquilinos";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilino
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
            return inquilinos;
        }

        public Inquilino? GetById(int id)
        {
            Inquilino? inquilino = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Dni, Nombre, Apellido, Email, Telefono FROM Inquilinos WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino
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
            return inquilino;
        }

        public void Create(Inquilino inquilino)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "INSERT INTO Inquilinos (Dni, Nombre, Apellido, Email, Telefono) VALUES (@Dni, @Nombre, @Apellido, @Email, @Telefono)";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@Email", inquilino.Email);
                    command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(int id, Inquilino inquilino)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "UPDATE Inquilinos SET Dni = @Dni, Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Telefono = @Telefono WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Dni", inquilino.Dni);
                    command.Parameters.AddWithValue("@Nombre", inquilino.Nombre);
                    command.Parameters.AddWithValue("@Apellido", inquilino.Apellido);
                    command.Parameters.AddWithValue("@Email", inquilino.Email);
                    command.Parameters.AddWithValue("@Telefono", inquilino.Telefono);
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
                string sql = "DELETE FROM Inquilinos WHERE Id = @Id";
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
