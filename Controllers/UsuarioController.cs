
using InmobiliariaWebApp.Models;
using InmobiliariaWebApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using BCrypt.Net;

namespace InmobiliariaWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public UsuarioController(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Clave)
        {
            var usuario = _usuarioRepository.GetByEmail(Email);

            if (usuario == null)
            {
                TempData["Error"] = "El email o la contraseña son incorrectos.";
                return View();
            }

            bool passwordIsValid = false;
            bool needsUpgrade = false;

            try
            {
                if (usuario.Clave.StartsWith("$2"))
                {
                    passwordIsValid = BCrypt.Net.BCrypt.Verify(Clave, usuario.Clave);
                }
                else
                {
                    if (usuario.Clave == Clave)
                    {
                        passwordIsValid = true;
                        needsUpgrade = true;
                    }
                }
            }
            catch (SaltParseException)
            {
                 if (usuario.Clave == Clave)
                    {
                        passwordIsValid = true;
                        needsUpgrade = true;
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
                _usuarioRepository.UpdatePassword(usuario.Id, hashedNewPassword);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
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
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }
            var usuario = _usuarioRepository.GetById(userId);
            if (usuario == null) return NotFound();
            return View(usuario);
        }

        [Authorize]
        [HttpPost]
        public IActionResult Perfil(Usuario usuario)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }
            usuario.Id = userId;
            try
            {
                _usuarioRepository.UpdateProfile(usuario);
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
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }
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
                _usuarioRepository.UpdateAvatar(userId, avatarUrl);
            }
            return RedirectToAction(nameof(Perfil));
        }

        [Authorize]
        [HttpPost]
        public IActionResult CambiarPassword(string claveActual, string claveNueva, string confirmarClave)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }

            if (string.IsNullOrEmpty(claveNueva) || claveNueva != confirmarClave)
            {
                TempData["Error"] = "La nueva contraseña no coincide con su confirmación.";
                return RedirectToAction(nameof(Perfil));
            }

            var usuario = _usuarioRepository.GetById(userId);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(claveActual, usuario.Clave))
            {
                TempData["Error"] = "La contraseña actual es incorrecta.";
                return RedirectToAction(nameof(Perfil));
            }

            try
            {
                var hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(claveNueva);
                _usuarioRepository.UpdatePassword(userId, hashedNewPassword);
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
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdValue, out var userId))
            {
                return Unauthorized();
            }
            try
            {
                _usuarioRepository.RemoveAvatar(userId);
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
