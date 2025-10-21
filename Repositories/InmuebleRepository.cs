using InmobiliariaWebApp.Models;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;

namespace InmobiliariaWebApp.Repositories
{
    public class InmuebleRepository : RepositoryBase, IInmuebleRepository
    {
        public InmuebleRepository(IConfiguration configuration) : base(configuration) { }

        public List<Inmueble> GetAll()
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT i.Id, i.Direccion, i.Ambientes, i.Uso, i.Precio, i.Disponible, i.PropietarioId, i.TipoInmuebleId,
                           p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                           t.Nombre AS TipoNombre
                    FROM Inmuebles i
                    INNER JOIN Propietarios p ON i.PropietarioId = p.Id
                    INNER JOIN TiposInmuebles t ON i.TipoInmuebleId = t.Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                                Dueño = new Propietario { Nombre = reader.GetString("PropietarioNombre"), Apellido = reader.GetString("PropietarioApellido") },
                                Tipo = new TipoInmueble { Nombre = reader.GetString("TipoNombre") }
                            });
                        }
                    }
                }
            }
            return inmuebles;
        }

        public Inmueble? GetById(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"SELECT i.Id, i.Direccion, i.Ambientes, i.Uso, i.Precio, i.Disponible, i.PropietarioId, i.TipoInmuebleId, p.Nombre AS PropietarioNombre, p.Apellido as PropietarioApellido, t.Nombre as TipoNombre 
                                FROM Inmuebles i 
                                JOIN Propietarios p ON i.PropietarioId = p.Id 
                                JOIN TiposInmuebles t on i.TipoInmuebleId = t.Id
                                WHERE i.Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDecimal("Precio"),
                                Disponible = reader.GetBoolean("Disponible"),
                                PropietarioId = reader.GetInt32("PropietarioId"),
                                TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                                Dueño = new Propietario { Nombre = reader.GetString("PropietarioNombre"), Apellido = reader.GetString("PropietarioApellido") },
                                Tipo = new TipoInmueble { Nombre = reader.GetString("TipoNombre") }
                            };
                        }
                    }
                }
            }
            return inmueble;
        }

        public void Create(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Inmuebles (Direccion, Ambientes, Uso, Precio, Disponible, PropietarioId, TipoInmuebleId) VALUES (@Direccion, @Ambientes, @Uso, @Precio, @Disponible, @PropietarioId, @TipoInmuebleId)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@Precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@Disponible", inmueble.Disponible);
                    command.Parameters.AddWithValue("@PropietarioId", inmueble.PropietarioId);
                    command.Parameters.AddWithValue("@TipoInmuebleId", inmueble.TipoInmuebleId);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(Inmueble inmueble)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Inmuebles SET Direccion = @Direccion, Ambientes = @Ambientes, Uso = @Uso, Precio = @Precio, Disponible = @Disponible, PropietarioId = @PropietarioId, TipoInmuebleId = @TipoInmuebleId WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                    command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                    command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                    command.Parameters.AddWithValue("@Precio", inmueble.Precio);
                    command.Parameters.AddWithValue("@Disponible", inmueble.Disponible);
                    command.Parameters.AddWithValue("@PropietarioId", inmueble.PropietarioId);
                    command.Parameters.AddWithValue("@TipoInmuebleId", inmueble.TipoInmuebleId);
                    command.Parameters.AddWithValue("@Id", inmueble.Id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "DELETE FROM Inmuebles WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool Exists(int id)
        {
            bool exists = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT 1 FROM Inmuebles WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    exists = command.ExecuteScalar() != null;
                }
            }
            return exists;
        }

        public List<Propietario> GetPropietarios()
        {
            var propietarios = new List<Propietario>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Nombre, Apellido FROM Propietarios";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            propietarios.Add(new Propietario { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") });
                        }
                    }
                }
            }
            return propietarios;
        }

        public List<TipoInmueble> GetTiposInmueble()
        {
            var tipos = new List<TipoInmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Nombre FROM TiposInmuebles";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipos.Add(new TipoInmueble { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre") });
                        }
                    }
                }
            }
            return tipos;
        }
    }
}
