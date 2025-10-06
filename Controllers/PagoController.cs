using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PagoController : Controller
    {
        private readonly Conexion _conexion;

        public PagoController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        private int ObtenerUsuarioIdActual()
        {
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

        public IActionResult Index(int id)
        {
            ViewBag.ContratoId = id;
            // Lógica para mostrar pagos
            return View(new List<Pago>());
        }

        public IActionResult Create(int id)
        {
            ViewBag.ContratoId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Pago pago)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    pago.UsuarioIdCreador = ObtenerUsuarioIdActual();
                    string sql = "INSERT INTO Pagos (NumeroPago, ContratoId, FechaPago, Importe, Detalle, Estado, UsuarioIdCreador) VALUES (@NumeroPago, @ContratoId, @FechaPago, @Importe, @Detalle, @Estado, @UsuarioIdCreador)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                        command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);
                        command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                        command.Parameters.AddWithValue("@Importe", pago.Importe);
                        command.Parameters.AddWithValue("@Detalle", pago.Detalle);
                        command.Parameters.AddWithValue("@Estado", "Vigente");
                        command.Parameters.AddWithValue("@UsuarioIdCreador", pago.UsuarioIdCreador);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index), new { id = pago.ContratoId });
            }
            catch { return View(pago); }
        }

        [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            // Lógica para buscar el pago
            return View();
        }
        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult DeleteConfirmed(int id)
        {
            int contratoId = 0;
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sqlSelect = "SELECT ContratoId FROM Pagos WHERE Id = @Id";
                    using(var command = new MySqlCommand(sqlSelect, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        var result = command.ExecuteScalar();
                        if(result != null) contratoId = Convert.ToInt32(result);
                    }

                    string sqlUpdate = "UPDATE Pagos SET Estado = @Estado, UsuarioIdAnulador = @UsuarioIdAnulador WHERE Id = @Id";
                    using (var command = new MySqlCommand(sqlUpdate, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Estado", "Anulado");
                        command.Parameters.AddWithValue("@UsuarioIdAnulador", ObtenerUsuarioIdActual());
                        command.Parameters.AddWithValue("@Id", id);
                        command.ExecuteNonQuery();
                    }
                }
                return RedirectToAction(nameof(Index), new { id = contratoId });
            }
            catch { return View(); }
        }
    }
}