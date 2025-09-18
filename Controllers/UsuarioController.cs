using InmobiliariaWebApp.Data; 
using InmobiliariaWebApp.Models; 
using Microsoft.AspNetCore.Mvc;

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
    }
}