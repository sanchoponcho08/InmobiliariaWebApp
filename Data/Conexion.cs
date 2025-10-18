using System.Data;
using MySqlConnector;

namespace InmobiliariaWebApp.Data
{
    public class Conexion
    {
        private readonly string connectionString;

        public Conexion(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        public IDbConnection TraerConexion()
        {
            return new MySqlConnection(connectionString);
        }
    }
}