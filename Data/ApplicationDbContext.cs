using Microsoft.EntityFrameworkCore;
using InmobiliariaWebApp.Models;

namespace InmobiliariaWebApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }


        public ApplicationDbContext()
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Propietario> Propietarios { get; set; }
        public DbSet<Inquilino> Inquilinos { get; set; }
        public DbSet<TipoInmueble> TiposInmuebles { get; set; }
        public DbSet<Inmueble> Inmuebles { get; set; }

    
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                IConfigurationRoot configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json")
                    .Build();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 21)));
            }
        }
    }
}