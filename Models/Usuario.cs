namespace InmobiliariaWebApp.Models 
{
    public class Usuario 
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public string Clave { get; set; }
        public string Rol { get; set; }
        public string? Avatar { get; set; }
    }
}