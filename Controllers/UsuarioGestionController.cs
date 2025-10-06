using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;

namespace InmobiliariaWebApp.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class UsuarioGestionController : Controller
    {
        private readonly Conexion _conexion;

        public UsuarioGestionController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Index()
        {
            var usuarios = new List<Usuario>();
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Rol FROM Usuarios";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            usuarios.Add(new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Rol = reader.GetString("Rol")
                            });
                        }
                    }
                }
            }
            return View(usuarios);
        }
        
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Usuario usuario)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "INSERT INTO Usuarios (Nombre, Apellido, Email, Clave, Rol) VALUES (@Nombre, @Apellido, @Email, @Clave, @Rol)";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Email", usuario.Email);
                        command.Parameters.AddWithValue("@Clave", usuario.Clave);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol);
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
            Usuario? usuario = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Rol FROM Usuarios WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Rol = reader.GetString("Rol")
                            };
                        }
                    }
                }
            }
            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Usuario usuario)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Nombre = @Nombre, Apellido = @Apellido, Email = @Email, Rol = @Rol WHERE Id = @Id";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Email", usuario.Email);
                        command.Parameters.AddWithValue("@Rol", usuario.Rol);
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
            Usuario? usuario = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Rol FROM Usuarios WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            usuario = new Usuario
                            {
                                Id = reader.GetInt32("Id"),
                                Nombre = reader.GetString("Nombre"),
                                Apellido = reader.GetString("Apellido"),
                                Email = reader.GetString("Email"),
                                Rol = reader.GetString("Rol")
                            };
                        }
                    }
                }
            }
            return usuario == null ? NotFound() : View(usuario);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "DELETE FROM Usuarios WHERE Id = @Id";
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