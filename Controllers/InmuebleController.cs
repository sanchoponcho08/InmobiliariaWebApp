using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class InmuebleController : Controller
    {
        private readonly Conexion _conexion;

        public InmuebleController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
        {
            var inmuebles = new List<Inmueble>();
            var propietarios = new Dictionary<int, Propietario>();
            var tipos = new Dictionary<int, TipoInmueble>();

            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();

                string sqlPropietarios = "SELECT Id, Nombre, Apellido FROM Propietarios";
                using (var command = new MySqlCommand(sqlPropietarios, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios[reader.GetInt32("Id")] = new Propietario { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") };
                    }
                }

                string sqlTipos = "SELECT Id, Nombre FROM TiposInmuebles";
                using (var command = new MySqlCommand(sqlTipos, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tipos[reader.GetInt32("Id")] = new TipoInmueble { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre") };
                    }
                }

                string sqlInmuebles = "SELECT Id, Direccion, Uso, TipoInmuebleId, Ambientes, Precio, Coordenadas, Disponible, PropietarioId FROM Inmuebles";
                using (var command = new MySqlCommand(sqlInmuebles, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var inmueble = new Inmueble
                        {
                            Id = reader.GetInt32("Id"),
                            Direccion = reader.GetString("Direccion"),
                            Uso = reader.GetString("Uso"),
                            TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                            Ambientes = reader.GetInt32("Ambientes"),
                            Precio = reader.GetDecimal("Precio"),
                            Coordenadas = reader.GetString("Coordenadas"),
                            Disponible = reader.GetBoolean("Disponible"),
                            PropietarioId = reader.GetInt32("PropietarioId")
                        };

                        if (propietarios.ContainsKey(inmueble.PropietarioId))
                        {
                            inmueble.Dueño = propietarios[inmueble.PropietarioId];
                        }

                        if (tipos.ContainsKey(inmueble.TipoInmuebleId))
                        {
                            inmueble.Tipo = tipos[inmueble.TipoInmuebleId];
                        }

                        inmuebles.Add(inmueble);
                    }
                }
            }
            return View(inmuebles);
        }

        public IActionResult Create()
        {
            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();

                var propietarios = new List<Propietario>();
                string sqlPropietarios = "SELECT Id, Nombre, Apellido FROM Propietarios";
                using (var command = new MySqlCommand(sqlPropietarios, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") });
                    }
                }

                var tipos = new List<TipoInmueble>();
                string sqlTipos = "SELECT Id, Nombre FROM TiposInmuebles";
                using (var command = new MySqlCommand(sqlTipos, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tipos.Add(new TipoInmueble { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre") });
                    }
                }

                ViewData["PropietarioId"] = new SelectList(propietarios, "Id", "NombreCompleto");
                ViewData["TipoInmuebleId"] = new SelectList(tipos, "Id", "Nombre");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inmueble inmueble)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "INSERT INTO Inmuebles (Direccion, Uso, TipoInmuebleId, Ambientes, Precio, Coordenadas, Disponible, PropietarioId) VALUES (@Direccion, @Uso, @TipoInmuebleId, @Ambientes, @Precio, @Coordenadas, @Disponible, @PropietarioId)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                        command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                        command.Parameters.AddWithValue("@TipoInmuebleId", inmueble.TipoInmuebleId);
                        command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                        command.Parameters.AddWithValue("@Precio", inmueble.Precio);
                        command.Parameters.AddWithValue("@Coordenadas", inmueble.Coordenadas);
                        command.Parameters.AddWithValue("@Disponible", inmueble.Disponible);
                        command.Parameters.AddWithValue("@PropietarioId", inmueble.PropietarioId);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public IActionResult Edit(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();

                string sql = "SELECT Id, Direccion, Uso, TipoInmuebleId, Ambientes, Precio, Coordenadas, Disponible, PropietarioId FROM Inmuebles WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inmueble = new Inmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Direccion = reader.GetString("Direccion"),
                                Uso = reader.GetString("Uso"),
                                TipoInmuebleId = reader.GetInt32("TipoInmuebleId"),
                                Ambientes = reader.GetInt32("Ambientes"),
                                Precio = reader.GetDecimal("Precio"),
                                Coordenadas = reader.GetString("Coordenadas"),
                                Disponible = reader.GetBoolean("Disponible"),
                                PropietarioId = reader.GetInt32("PropietarioId")
                            };
                        }
                    }
                }

                if (inmueble == null) return NotFound();

                var propietarios = new List<Propietario>();
                string sqlPropietarios = "SELECT Id, Nombre, Apellido FROM Propietarios";
                using (var command = new MySqlCommand(sqlPropietarios, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") });
                    }
                }

                var tipos = new List<TipoInmueble>();
                string sqlTipos = "SELECT Id, Nombre FROM TiposInmuebles";
                using (var command = new MySqlCommand(sqlTipos, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tipos.Add(new TipoInmueble { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre") });
                    }
                }

                ViewData["PropietarioId"] = new SelectList(propietarios, "Id", "NombreCompleto", inmueble.PropietarioId);
                ViewData["TipoInmuebleId"] = new SelectList(tipos, "Id", "Nombre", inmueble.TipoInmuebleId);
            }
            return View(inmueble);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inmueble inmueble)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Inmuebles SET Direccion = @Direccion, Uso = @Uso, TipoInmuebleId = @TipoInmuebleId, Ambientes = @Ambientes, Precio = @Precio, Coordenadas = @Coordenadas, Disponible = @Disponible, PropietarioId = @PropietarioId WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Direccion", inmueble.Direccion);
                        command.Parameters.AddWithValue("@Uso", inmueble.Uso);
                        command.Parameters.AddWithValue("@TipoInmuebleId", inmueble.TipoInmuebleId);
                        command.Parameters.AddWithValue("@Ambientes", inmueble.Ambientes);
                        command.Parameters.AddWithValue("@Precio", inmueble.Precio);
                        command.Parameters.AddWithValue("@Coordenadas", inmueble.Coordenadas);
                        command.Parameters.AddWithValue("@Disponible", inmueble.Disponible);
                        command.Parameters.AddWithValue("@PropietarioId", inmueble.PropietarioId);
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
        
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            Inmueble? inmueble = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT i.Id, i.Direccion, i.Uso, i.Precio, 
                        p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                        t.Nombre AS TipoNombre
                    FROM Inmuebles i
                    JOIN Propietarios p ON i.PropietarioId = p.Id
                    JOIN TiposInmuebles t ON i.TipoInmuebleId = t.Id
                    WHERE i.Id = @Id";

                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
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
                                Uso = reader.GetString("Uso"),
                                Precio = reader.GetDecimal("Precio"),
                                Dueño = new Propietario { Nombre = reader.GetString("PropietarioNombre"), Apellido = reader.GetString("PropietarioApellido") },
                                Tipo = new TipoInmueble { Nombre = reader.GetString("TipoNombre") }
                            };
                        }
                    }
                }
            }
    return inmueble == null ? NotFound() : View(inmueble);
}

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "DELETE FROM Inmuebles WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}