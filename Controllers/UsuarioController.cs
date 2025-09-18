using InmobiliariaWebApp.Data; 
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace InmobiliariaWebApp.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;


        public UsuarioController(ApplicationDbContext context)
        {
            _context = context;
        }

        // formulario de registro 
        public IActionResult Register()
        {
            return View();
        }

        //  recibe los datos del formulario 
        [HttpPost]
        public async Task<IActionResult> Register(Usuario usuario)
        {
            // Asignamos un rol por defecto
            usuario.Rol = "Empleado";

            // Guardamos el nuevo usuario en la base de datos
            _context.Add(usuario);
            await _context.SaveChangesAsync();

            // Redirigimos al usuario a la página de login
            return RedirectToAction("Login");
        }
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Clave)
        {
    // Buscamos el usuario en la base de datos por su email
    var usuario = await _context.Usuarios
        .FirstOrDefaultAsync(u => u.Email == Email);

    if (usuario == null || usuario.Clave != Clave)
    {
        // mensaje de error que la vista podrá mostrar
        TempData["Error"] = "El email o la contraseña son incorrectos.";
        return View();
    }


    var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, usuario.Email),
        new Claim("FullName", $"{usuario.Nombre} {usuario.Apellido}"),
        new Claim(ClaimTypes.Role, usuario.Rol),
    };

    var claimsIdentity = new ClaimsIdentity(
        claims, CookieAuthenticationDefaults.AuthenticationScheme);

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
    }
}