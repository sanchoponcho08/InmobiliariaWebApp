using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class InquilinoController : Controller
    {
        private readonly Conexion _conexion;

        public InquilinoController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
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
            return View(inquilinos);
        }

        public IActionResult Details(int id)
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
            return inquilino == null ? NotFound() : View(inquilino);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Inquilino inquilino)
        {
            try
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
                TempData["Success"] = "Inquilino creado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al crear el inquilino.";
                return View(inquilino);
            }
        }

        public IActionResult Edit(int id)
        {
            return Details(id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Inquilino inquilino)
        {
            try
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
                TempData["Success"] = "Inquilino actualizado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al actualizar el inquilino.";
                return View(inquilino);
                
            }
        }

      [Authorize(Roles = "Administrador")]
        public IActionResult Delete(int id)
        {
            Inquilino? inquilino = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Dni, Nombre, Apellido FROM Inquilinos WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            inquilino = new Inquilino { Id = reader.GetInt32("Id"), Dni = reader.GetString("Dni"), Nombre = reader.GetString("Nombre"), Apellido = reader.GetString("Apellido") };
                        }
                    }
                }
            }
            return inquilino == null ? NotFound() : View(inquilino);
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
                    string sql = "DELETE FROM Inquilinos WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Id", id);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Inquilino eliminado exitosamente.";
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                TempData["Error"] = "Ocurrió un error al eliminar el inquilino.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}