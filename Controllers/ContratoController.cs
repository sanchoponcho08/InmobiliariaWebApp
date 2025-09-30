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
    public class ContratoController : Controller
    {
        private readonly Conexion _conexion;

        public ContratoController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
        {
            var contratos = new List<Contrato>();
            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, c.InquilinoId, c.InmuebleId,
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           im.Direccion AS InmuebleDireccion,
                           p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido
                    FROM Contratos c
                    LEFT JOIN Inquilinos i ON c.InquilinoId = i.Id
                    LEFT JOIN Inmuebles im ON c.InmuebleId = im.Id
                    LEFT JOIN Propietarios p ON im.PropietarioId = p.Id";

                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
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
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
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
                            });
                        }
                    }
                }
            }
            return View(contratos);
        }

        public IActionResult Details(int id)
        {
            Contrato? contrato = null;
            // Lógica similar a Index pero con WHERE Id = @Id para buscar uno solo
            // y popular las propiedades de navegación.
            return contrato == null ? NotFound() : View(contrato);
        }

        public IActionResult Create()
        {
            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();
                
                var inquilinos = new List<Inquilino>();
                string sqlInquilinos = "SELECT Id, Nombre, Apellido FROM Inquilinos";
                using (var command = new MySqlCommand(sqlInquilinos, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inquilinos.Add(new Inquilino { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") });
                    }
                }

                var inmuebles = new List<Inmueble>();
                string sqlInmuebles = "SELECT Id, Direccion FROM Inmuebles";
                using (var command = new MySqlCommand(sqlInmuebles, (MySqlConnection)connection))
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        inmuebles.Add(new Inmueble { Id = reader.GetInt32("Id"), Direccion = reader.GetString("Direccion") });
                    }
                }

                ViewData["InquilinoId"] = new SelectList(inquilinos, "Id", "NombreCompleto");
                ViewData["InmuebleId"] = new SelectList(inmuebles, "Id", "Direccion");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Contrato contrato)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "INSERT INTO Contratos (InquilinoId, InmuebleId, FechaInicio, FechaFin, MontoAlquiler, FechaRescision, Multa) VALUES (@InquilinoId, @InmuebleId, @FechaInicio, @FechaFin, @MontoAlquiler, NULL, 0)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                        command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                        command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@MontoAlquiler", contrato.MontoAlquiler);
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
            Contrato? contrato = null;
            // Lógica para buscar el contrato por ID (similar a Details)
            // Lógica para cargar ViewData para los dropdowns (similar a Create)
            return contrato == null ? NotFound() : View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
             try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Contratos SET InquilinoId = @InquilinoId, InmuebleId = @InmuebleId, FechaInicio = @FechaInicio, FechaFin = @FechaFin, MontoAlquiler = @MontoAlquiler WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                        command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                        command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@MontoAlquiler", contrato.MontoAlquiler);
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
        
        public IActionResult Delete(int id)
        {
            // Lógica para buscar el contrato por ID (similar a Details)
             return Details(id);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    // Primero borrar pagos asociados si existen
                    string sqlPagos = "DELETE FROM Pagos WHERE ContratoId = @ContratoId";
                    using(var command = new MySqlCommand(sqlPagos, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }

                    // Luego borrar el contrato
                    string sqlContrato = "DELETE FROM Contratos WHERE Id = @Id";
                    using (var command = new MySqlCommand(sqlContrato, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
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

        public IActionResult Terminar(int id)
        {
            Contrato? contrato = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido, im.Direccion AS InmuebleDireccion FROM Contratos c JOIN Inquilinos i ON c.InquilinoId = i.Id JOIN Inmuebles im ON c.InmuebleId = im.Id WHERE c.Id = @Id";
                using(var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using(var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            contrato = new Contrato{
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                                Inquilino = new Inquilino { Nombre = reader.GetString("InquilinoNombre"), Apellido = reader.GetString("InquilinoApellido") },
                                Inmueble = new Inmueble { Direccion = reader.GetString("InmuebleDireccion") }
                            };
                        }
                    }
                }
            }

            if(contrato == null) return NotFound();

            var totalMeses = (contrato.FechaFin.Year - contrato.FechaInicio.Year) * 12 + contrato.FechaFin.Month - contrato.FechaInicio.Month;
            var mesesCumplidos = (DateTime.Now.Year - contrato.FechaInicio.Year) * 12 + DateTime.Now.Month - contrato.FechaInicio.Month;

            if (mesesCumplidos < totalMeses / 2.0)
            {
                ViewBag.Multa = contrato.MontoAlquiler * 2;
            }
            else
            {
                ViewBag.Multa = contrato.MontoAlquiler;
            }
            
            return View("Terminar", contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id, decimal Multa)
        {
            using (var connection = _conexion.TraerConexion())
            {
                connection.Open();
                string sqlContrato = "UPDATE Contratos SET FechaRescision = @FechaRescision, Multa = @Multa WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlContrato, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@FechaRescision", DateTime.Now);
                    command.Parameters.AddWithValue("@Multa", Multa);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string sqlPago = "INSERT INTO Pagos (NumeroPago, ContratoId, FechaPago, Importe, Detalle, Estado) VALUES (@NumeroPago, @ContratoId, @FechaPago, @Importe, @Detalle, @Estado)";
                using (var command = new MySqlCommand(sqlPago, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@NumeroPago", 99);
                    command.Parameters.AddWithValue("@ContratoId", id);
                    command.Parameters.AddWithValue("@FechaPago", DateTime.Now);
                    command.Parameters.AddWithValue("@Importe", Multa);
                    command.Parameters.AddWithValue("@Detalle", "Pago de multa por rescisión anticipada");
                    command.Parameters.AddWithValue("@Estado", "Vigente");
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}