using System.Data;
using MySql.Data.MySqlClient;

namespace InmobiliariaWebApp.Data
{
    public class Conexion
    {
        private readonly string connectionString;
        private readonly IConfiguration configuration;

        public Conexion(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.connectionString = configuration["ConnectionStrings:DefaultConnection"];
        }

        public IDbConnection TraerConexion()
        {
            return new MySqlConnection(connectionString);
        }
    }
}