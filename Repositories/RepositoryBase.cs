using System.Data;
using MySql.Data.MySqlClient;

namespace InmobiliariaWebApp.Repositories
{
    public abstract class RepositoryBase
    {
        protected readonly string connectionString;

        public RepositoryBase(IConfiguration configuration)
        {
            // La cadena de conexión se obtiene de appsettings.json
            connectionString = configuration.GetConnectionString("DefaultConnection");
        }
    }
}
