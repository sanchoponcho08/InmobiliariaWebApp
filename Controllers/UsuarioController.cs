using InmobiliariaWebApp.Data; 
using InmobiliariaWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

    if (usuario == null)
    {
        // Si no se encuentra el usuario, volvemos a la vista de login
        //  agregar un mensaje de error despues
        return View();
    }

    // Comparamos la contraseña del formulario con la guardada en la BD texto luego modificar
    if (usuario.Clave != Clave)
    {
        
        return View();
    }

    
    return RedirectToAction("Index", "Home");
}
        
    }
}