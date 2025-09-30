using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class TipoInmuebleController : Controller
    {
        private readonly Conexion _conexion;

        public TipoInmuebleController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
        {
            var tipos = new List<TipoInmueble>();
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre FROM TiposInmuebles";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tipos.Add(new TipoInmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre")
                            });
                        }
                    }
                }
            }
            return View(tipos);
        }

        public IActionResult Details(int id)
        {
            TipoInmueble? tipo = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre FROM TiposInmuebles WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            tipo = new TipoInmueble
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre")
                            };
                        }
                    }
                }
            }
            return tipo == null ? NotFound() : View(tipo);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(TipoInmueble tipoInmueble)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "INSERT INTO TiposInmuebles (Nombre) VALUES (@Nombre)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", tipoInmueble.Nombre);
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
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, TipoInmueble tipoInmueble)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE TiposInmuebles SET Nombre = @Nombre WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", tipoInmueble.Nombre);
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
                    string sql = "DELETE FROM TiposInmuebles WHERE Id = @Id";
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