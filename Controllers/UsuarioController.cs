using InmobiliariaWebApp.Data;
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using System.Data;
using System.IO;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;

namespace InmobiliariaWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly Conexion _conexion;

        public UsuarioController(IConfiguration configuration)
        {
            _conexion = new Conexion(configuration);
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Clave)
        {
            Usuario? usuario = null;
            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Clave, Rol FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", Email);
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
                                Clave = reader.GetString("Clave"),
                                Rol = reader.GetString("Rol")
                            };
                        }
                    }
                }
            }

            if (usuario == null)
            {
                TempData["Error"] = "El email o la contraseña son incorrectos.";
                return View();
            }

            bool passwordIsValid = false;
            bool needsUpgrade = false;

            try
            {
                // Si la clave parece un hash, se verifica. Si no, se trata como texto plano.
                if (usuario.Clave.StartsWith("$2"))
                {
                    passwordIsValid = BCrypt.Net.BCrypt.Verify(Clave, usuario.Clave);
                }
                else
                {
                    if (usuario.Clave == Clave)
                    {
                        passwordIsValid = true;
                        needsUpgrade = true; // Marcar para actualizar a hash
                    }
                }
            }
            catch (SaltParseException)
            {
                // Si BCrypt falla al parsear, asumimos que es texto plano y comparamos.
                 if (usuario.Clave == Clave)
                    {
                        passwordIsValid = true;
                        needsUpgrade = true; // Marcar para actualizar a hash
                    }
            }


            if (!passwordIsValid)
            {
                TempData["Error"] = "El email o la contraseña son incorrectos.";
                return View();
            }

            if (needsUpgrade)
            {
                var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(Clave);
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Clave = @ClaveNueva WHERE Email = @Email";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@ClaveNueva", hashedNewPassword);
                        command.Parameters.AddWithValue("@Email", usuario.Email);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.Email),
                new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}"),
                new Claim(ClaimTypes.Role, usuario.Rol),
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        public IActionResult Perfil()
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name)) return Unauthorized();
            Usuario? usuario = null;
            var userEmail = User.Identity.Name;

            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Id, Nombre, Apellido, Email, Avatar FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", userEmail);
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
                                Avatar = reader.IsDBNull("Avatar") ? null : reader.GetString("Avatar")
                            };
                        }
                    }
                }
            }
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Perfil(Usuario usuario)
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name)) return Unauthorized();
            var userEmail = User.Identity.Name;
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Nombre = @Nombre, Apellido = @Apellido WHERE Email = @Email";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Nombre", usuario.Nombre);
                        command.Parameters.AddWithValue("@Apellido", usuario.Apellido);
                        command.Parameters.AddWithValue("@Email", userEmail);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Perfil actualizado correctamente.";
                return RedirectToAction(nameof(Perfil));
            }
            catch
            {
                TempData["Error"] = "No se pudo actualizar el perfil.";
                return View(usuario);
            }
        }

        [Authorize]
        [HttpPost]
        public IActionResult ActualizarAvatar(IFormFile avatar)
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name)) return Unauthorized();
            var userEmail = User.Identity.Name;
            if (avatar != null && avatar.Length > 0)
            {
                var uploadsDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/avatars");
                if (!Directory.Exists(uploadsDir))
                {
                    Directory.CreateDirectory(uploadsDir);
                }
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(avatar.FileName);
                var filePath = Path.Combine(uploadsDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    avatar.CopyTo(fileStream);
                }

                var avatarUrl = "/uploads/avatars/" + uniqueFileName;

                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Avatar = @Avatar WHERE Email = @Email";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Avatar", avatarUrl);
                        command.Parameters.AddWithValue("@Email", userEmail);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction(nameof(Perfil));
        }
        [Authorize]
        [HttpPost]
        public IActionResult CambiarPassword(string claveActual, string claveNueva, string confirmarClave)
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name)) return Unauthorized();
            var userEmail = User.Identity.Name;

            if (string.IsNullOrEmpty(claveNueva) || claveNueva != confirmarClave)
            {
                TempData["Error"] = "La nueva contraseña no coincide con su confirmación.";
                return RedirectToAction(nameof(Perfil));
            }

            using (var connection = _conexion.TraerConexion())
            {
                string sql = "SELECT Clave FROM Usuarios WHERE Email = @Email";
                using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                {
                    command.Parameters.AddWithValue("@Email", userEmail);
                    connection.Open();
                    var storedPassword = command.ExecuteScalar() as string;

                    if (storedPassword == null || !BCrypt.Net.BCrypt.Verify(claveActual, storedPassword))
                    {
                        TempData["Error"] = "La contraseña actual es incorrecta.";
                        return RedirectToAction(nameof(Perfil));
                    }
                }
            }

            try
            {
                var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(claveNueva);
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Clave = @ClaveNueva WHERE Email = @Email";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@ClaveNueva", hashedNewPassword);
                        command.Parameters.AddWithValue("@Email", userEmail);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Contraseña actualizada correctamente.";
            }
            catch
            {
                TempData["Error"] = "No se pudo actualizar la contraseña.";
            }

            return RedirectToAction(nameof(Perfil));
        }
        
        [Authorize]
        [HttpPost]
        public IActionResult QuitarAvatar()
        {
            if (User.Identity == null || string.IsNullOrEmpty(User.Identity.Name)) return Unauthorized();
            var userEmail = User.Identity.Name;
            try
            {
                using (var connection = _conexion.TraerConexion())
                {
                    string sql = "UPDATE Usuarios SET Avatar = NULL WHERE Email = @Email";
                    using (var command = new MySqlCommand(sql, (MySqlConnection)connection))
                    {
                        command.Parameters.AddWithValue("@Email", userEmail);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
                TempData["Success"] = "Foto de perfil eliminada.";
            }
            catch
            {
                TempData["Error"] = "No se pudo eliminar la foto de perfil.";
            }
            return RedirectToAction(nameof(Perfil));
        }
    }
}
