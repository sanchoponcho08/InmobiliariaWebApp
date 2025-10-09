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

        private int ObtenerUsuarioIdActual()
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name))
            {
                return 0;
            }
            var email = User.Identity.Name;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    connection.Open();
                    var id = command.ExecuteScalar();
                    return id != null ? Convert.ToInt32(id) : 0;
                }
            }
        }

        private bool VerificarSuperposicion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int contratoId = 0)
        {
            bool seSuperpone = false;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT COUNT(*) FROM Contratos WHERE InmuebleId = @InmuebleId AND @FechaInicio < FechaFin AND @FechaFin > FechaInicio AND Id != @ContratoId";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.Parameters.AddWithValue("@ContratoId", contratoId);
                    connection.Open();
                    long count = (long)command.ExecuteScalar();
                    seSuperpone = count > 0;
                }
            }
            return seSuperpone;
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
            using (var connection = _conexion.TraerConexion())
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, c.FechaRescision, c.Multa,
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        im.Direccion AS InmuebleDireccion,
                        p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                        uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido,
                        ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido
                    FROM Contratos c
                    LEFT JOIN Inquilinos i ON c.InquilinoId = i.Id
                    LEFT JOIN Inmuebles im ON c.InmuebleId = im.Id
                    LEFT JOIN Propietarios p ON im.PropietarioId = p.Id
                    LEFT JOIN Usuarios uc ON c.UsuarioIdCreador = uc.Id
                    LEFT JOIN Usuarios ut ON c.UsuarioIdTerminador = ut.Id
                    WHERE c.Id = @Id";
                
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                                FechaRescision = reader.IsDBNull(reader.GetOrdinal("FechaRescision")) ? (DateTime?)null : reader.GetDateTime("FechaRescision"),
                                Multa = reader.GetDecimal("Multa"),
                                Inquilino = new Inquilino { Nombre = reader.GetString("InquilinoNombre"), Apellido = reader.GetString("InquilinoApellido") },
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.GetString("InmuebleDireccion"),
                                    Dueño = new Propietario { Nombre = reader.GetString("PropietarioNombre"), Apellido = reader.GetString("PropietarioApellido") }
                                },
                                Creador = reader.IsDBNull(reader.GetOrdinal("CreadorNombre")) ? null : new Usuario { Nombre = reader.GetString("CreadorNombre"), Apellido = reader.GetString("CreadorApellido") },
                                Terminador = reader.IsDBNull(reader.GetOrdinal("TerminadorNombre")) ? null : new Usuario { Nombre = reader.GetString("TerminadorNombre"), Apellido = reader.GetString("TerminadorApellido") }
                            };
                        }
                    }
                }
            }
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
            if (VerificarSuperposicion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin))
            {
                ModelState.AddModelError("", "Las fechas de este contrato se superponen con un contrato existente para el mismo inmueble.");

                using (var connection = _conexion.TraerConexion())
                {
                    connection.Open();
                    var inquilinos = new List<Inquilino>();
                    string sqlInquilinos = "SELECT Id, Nombre, Apellido FROM Inquilinos";
                    using (var cmd = new MySqlCommand(sqlInquilinos, (MySqlConnection)connection))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            inquilinos.Add(new Inquilino { Id = rdr.GetInt32("Id"), Nombre = rdr.GetString("Nombre"), Apellido = rdr.GetString("Apellido") });
                        }
                    }

                    var inmuebles = new List<Inmueble>();
                    string sqlInmuebles = "SELECT Id, Direccion FROM Inmuebles";
                    using (var cmd = new MySqlCommand(sqlInmuebles, (MySqlConnection)connection))
                    using (var rdr = cmd.ExecuteReader())
                    {
                        while (rdr.Read())
                        {
                            inmuebles.Add(new Inmueble { Id = rdr.GetInt32("Id"), Direccion = rdr.GetString("Direccion") });
                        }
                    }

                    ViewData["InquilinoId"] = new SelectList(inquilinos, "Id", "NombreCompleto", contrato.InquilinoId);
                    ViewData["InmuebleId"] = new SelectList(inmuebles, "Id", "Direccion", contrato.InmuebleId);
                }

                return View(contrato);
            }

            try
            {
                contrato.UsuarioIdCreador = ObtenerUsuarioIdActual();
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "INSERT INTO Contratos (InquilinoId, InmuebleId, FechaInicio, FechaFin, MontoAlquiler, UsuarioIdCreador, FechaRescision, Multa) VALUES (@InquilinoId, @InmuebleId, @FechaInicio, @FechaFin, @MontoAlquiler, @UsuarioIdCreador, NULL, 0)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                        command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                        command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                        command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                        command.Parameters.AddWithValue("@MontoAlquiler", contrato.MontoAlquiler);
                        command.Parameters.AddWithValue("@UsuarioIdCreador", contrato.UsuarioIdCreador);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Contrato creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al crear el contrato.";
                return View(contrato);
            }
        }

        public IActionResult Edit(int id)
        {
            Contrato? contrato = null;
            return contrato == null ? NotFound() : View(contrato);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Contrato contrato)
        {
             if (VerificarSuperposicion(contrato.InmuebleId, contrato.FechaInicio, contrato.FechaFin, id))
            {
                ModelState.AddModelError("", "Las fechas se superponen con un contrato existente para el mismo inmueble.");
                return View(contrato);
            }
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
                TempData["Success"] = "Contrato actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el contrato.";
                return View(contrato);
            }
        }
        
        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            if (!User.IsInRole("Administrador"))
            {
                return RedirectToAction("AccesoDenegado", "Home");
            }
            return Details(id);
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
                    connection.Open();
                    string sqlPagos = "DELETE FROM Pagos WHERE ContratoId = @ContratoId";
                    using (var command = new MySqlCommand(sqlPagos, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@ContratoId", id);
                        command.ExecuteNonQuery();
                    }
                    string sqlContrato = "DELETE FROM Contratos WHERE Id = @Id";
                    using (var command = new MySqlCommand(sqlContrato, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Contrato y sus pagos asociados han sido eliminados exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                 TempData["Error"] = "Ocurrió un error al eliminar el contrato.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }
        }

        public IActionResult Terminar(int id)
        {
            return View("Terminar", new Contrato());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Terminar(int id, decimal Multa)
        {
             using (var connection = _conexion.TraerConexion())
            {
                connection.Open();
                string sqlContrato = "UPDATE Contratos SET FechaRescision = @FechaRescision, Multa = @Multa, UsuarioIdTerminador = @UsuarioIdTerminador WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlContrato, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@FechaRescision", DateTime.Now);
                    command.Parameters.AddWithValue("@Multa", Multa);
                    command.Parameters.AddWithValue("@UsuarioIdTerminador", ObtenerUsuarioIdActual());
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string sqlPago = "INSERT INTO Pagos (NumeroPago, ContratoId, FechaPago, Importe, Detalle, Estado, UsuarioIdCreador) VALUES (@NumeroPago, @ContratoId, @FechaPago, @Importe, @Detalle, @Estado, @UsuarioIdCreador)";
                using (var command = new MySqlCommand(sqlPago, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@NumeroPago", 99);
                    command.Parameters.AddWithValue("@ContratoId", id);
                    command.Parameters.AddWithValue("@FechaPago", DateTime.Now);
                    command.Parameters.AddWithValue("@Importe", Multa);
                    command.Parameters.AddWithValue("@Detalle", "Pago de multa por rescisión anticipada");
                    command.Parameters.AddWithValue("@Estado", "Vigente");
                    command.Parameters.AddWithValue("@UsuarioIdCreador", ObtenerUsuarioIdActual());
                    command.ExecuteNonQuery();
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Renovar(int id)
        {
            var nuevoContrato = new Contrato();
            return View("Create", nuevoContrato);
        }
    }
}