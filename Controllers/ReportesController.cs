using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly Conexion _conexion;

        public ReportesController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult ContratosVigentes()
        {
            var contratos = new List<Contrato>();
            var fechaActual = DateTime.Now;

            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler,
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           im.Direccion AS InmuebleDireccion,
                           p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido
                    FROM Contratos c
                    JOIN Inquilinos i ON c.InquilinoId = i.Id
                    JOIN Inmuebles im ON c.InmuebleId = im.Id
                    JOIN Propietarios p ON im.PropietarioId = p.Id
                    WHERE @fechaActual BETWEEN c.FechaInicio AND c.FechaFin";

                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@fechaActual", fechaActual);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(MapContratoFromReader(reader));
                        }
                    }
                }
            }
            return View(contratos);
        }

        public IActionResult ContratosPorVencer(int dias = 30)
        {
            var contratos = new List<Contrato>();
            var fechaActual = DateTime.Now;
            var fechaLimite = fechaActual.AddDays(dias);
            ViewBag.Dias = dias;

            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler,
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           im.Direccion AS InmuebleDireccion,
                           p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido
                    FROM Contratos c
                    JOIN Inquilinos i ON c.InquilinoId = i.Id
                    JOIN Inmuebles im ON c.InmuebleId = im.Id
                    JOIN Propietarios p ON im.PropietarioId = p.Id
                    WHERE c.FechaFin BETWEEN @fechaActual AND @fechaLimite";

                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@fechaActual", fechaActual);
                    command.Parameters.AddWithValue("@fechaLimite", fechaLimite);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(MapContratoFromReader(reader));
                        }
                    }
                }
            }
            return View(contratos);
        }

        // Método para no repetir código
        private Contrato MapContratoFromReader(MySqlDataReader reader)
        {
            return new Contrato
            {
                Id = reader.GetInt32("Id"),
                FechaInicio = reader.GetDateTime("FechaInicio"),
                FechaFin = reader.GetDateTime("FechaFin"),
                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                Inquilino = new Inquilino
                {
                    Nombre = reader.GetString("InquilinoNombre"),
                    Apellido = reader.GetString("InquilinoApellido")
                },
                Inmueble = new Inmueble
                {
                    Direccion = reader.GetString("InmuebleDireccion"),
                    Dueño = new Propietario
                    {
                        Nombre = reader.GetString("PropietarioNombre"),
                        Apellido = reader.GetString("PropietarioApellido")
                    }
                }
            };
        }
        public IActionResult InmueblesPorPropietario(int propietarioId)
        {
            var propietarios = new List<Propietario>();
            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();
                string sqlPropietarios = "SELECT Id, Nombre, Apellido FROM Propietarios";
                using (var command = new MySqlCommand(sqlPropietarios, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        propietarios.Add(new Propietario { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") });
                    }
                }
                ViewData["Propietarios"] = new SelectList(propietarios, "Id", "NombreCompleto", propietarioId);
            }

            var inmuebles = new List<Inmueble>();
            if (propietarioId > 0)
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = @"
                SELECT i.Id, i.Direccion, i.Uso, i.Precio, i.Disponible, t.Nombre AS TipoNombre
                FROM Inmuebles i
                JOIN TiposInmuebles t ON i.TipoInmuebleId = t.Id
                WHERE i.PropietarioId = @propietarioId";

                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@propietarioId", propietarioId);
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                inmuebles.Add(new Inmueble
                                {
                                    Id = reader.GetInt32("Id"),
                                    Direccion = reader.GetString("Direccion"),
                                    Uso = reader.GetString("Uso"),
                                    Precio = reader.GetDecimal("Precio"),
                                    Disponible = reader.GetBoolean("Disponible"),
                                    Tipo = new TipoInmueble { Nombre = reader.GetString("TipoNombre") }
                                });
                            }
                        }
                    }
                }
            }
            return View(inmuebles);
        }
        public IActionResult ContratosPorInmueble(int inmuebleId)
    {
        var inmuebles = new List<Inmueble>();
        using (var connection = _conexion.TraerConexion())
        {
            connection.Open();
            string sqlInmuebles = "SELECT Id, Direccion FROM Inmuebles";
            using (var command = new MySqlCommand(sqlInmuebles, (MySqlConnection)connection))
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    inmuebles.Add(new Inmueble { Id = reader.GetInt32("Id"), Direccion = reader.GetString("Direccion") });
                }
            }
            ViewData["Inmuebles"] = new SelectList(inmuebles, "Id", "Direccion", inmuebleId);
        }

        var contratos = new List<Contrato>();
        if (inmuebleId > 0)
        {
            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido
                    FROM Contratos c
                    JOIN Inquilinos i ON c.InquilinoId = i.Id
                    WHERE c.InmuebleId = @inmuebleId";

                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@inmuebleId", inmuebleId);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                                Inquilino = new Inquilino { Nombre = reader.GetString("InquilinoNombre"), Apellido = reader.GetString("InquilinoApellido") }
                            });
                        }
                    }
                }
            }
        }
        return View(contratos);
        }    
    }
}