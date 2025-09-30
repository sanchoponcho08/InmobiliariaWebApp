using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize]
    public class PropietarioController : Controller
    {
        private readonly Conexion _conexion;

        public PropietarioController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
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
            return View(propietarios);
        }

        public IActionResult Details(int id)
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
            return propietario == null ? NotFound() : View(propietario);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Propietario propietario)
        {
            try
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
        public IActionResult Edit(int id, Propietario propietario)
        {
            try
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
                    string sql = "DELETE FROM Propietarios WHERE Id = @Id";
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