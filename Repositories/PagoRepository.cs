using InmobiliariaWebApp.Models;
using MySqlConnector;

namespace InmobiliariaWebApp.Repositories
{
    public class PagoRepository : RepositoryBase, IPagoRepository
    {
        public PagoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Pago> GetPagosByContratoId(int contratoId)
        {
            var pagos = new List<Pago>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sqlPagos = @"
                    SELECT p.Id, p.NumeroPago, p.FechaPago, p.Importe, p.Detalle, p.Estado,
                           uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido
                    FROM Pagos p
                    LEFT JOIN Usuarios uc ON p.UsuarioIdCreador = uc.Id
                    WHERE p.ContratoId = @ContratoId";
                using (var command = new MySqlCommand(sqlPagos, connection))
                {
                    connection.Open();
                    command.Parameters.AddWithValue("@ContratoId", contratoId);
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            pagos.Add(new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("NumeroPago"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString("Detalle"),
                                Estado = reader.GetString("Estado")!,
                                Creador = reader.IsDBNull(reader.GetOrdinal("CreadorNombre")) ? null : new Usuario { Nombre = reader.GetString("CreadorNombre")!, Apellido = reader.GetString("CreadorApellido")! }
                            });
                        }
                    }
                }
            }
            return pagos;
        }

        public Pago? GetById(int id)
        {
            Pago? pago = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                 string sql = @"
                    SELECT p.Id, p.NumeroPago, p.FechaPago, p.Importe, p.Detalle, p.Estado, p.ContratoId,
                        c.InquilinoId, c.InmuebleId,
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        im.Direccion AS InmuebleDireccion,
                        uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido,
                        ua.Nombre AS AnuladorNombre, ua.Apellido AS AnuladorApellido
                    FROM Pagos p
                    JOIN Contratos c ON p.ContratoId = c.Id
                    JOIN Inquilinos i ON c.InquilinoId = i.Id
                    JOIN Inmuebles im ON c.InmuebleId = im.Id
                    LEFT JOIN Usuarios uc ON p.UsuarioIdCreador = uc.Id
                    LEFT JOIN Usuarios ua ON p.UsuarioIdAnulador = ua.Id
                    WHERE p.Id = @Id";
                    
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            pago = new Pago
                            {
                                Id = reader.GetInt32("Id"),
                                NumeroPago = reader.GetInt32("NumeroPago"),
                                FechaPago = reader.GetDateTime("FechaPago"),
                                Importe = reader.GetDecimal("Importe"),
                                Detalle = reader.IsDBNull(reader.GetOrdinal("Detalle")) ? null : reader.GetString("Detalle"),
                                Estado = reader.GetString("Estado")!,
                                ContratoId = reader.GetInt32("ContratoId"),
                                Contrato = new Contrato
                                {
                                    Id = reader.GetInt32("ContratoId"),
                                    InquilinoId = reader.GetInt32("InquilinoId"),
                                    InmuebleId = reader.GetInt32("InmuebleId"),
                                    Inquilino = new Inquilino { Nombre = reader.GetString("InquilinoNombre")!, Apellido = reader.GetString("InquilinoApellido")! },
                                    Inmueble = new Inmueble { Direccion = reader.GetString("InmuebleDireccion")! }
                                },
                                Creador = reader.IsDBNull(reader.GetOrdinal("CreadorNombre")) ? null : new Usuario { Nombre = reader.GetString("CreadorNombre")!, Apellido = reader.GetString("CreadorApellido")! },
                                Anulador = reader.IsDBNull(reader.GetOrdinal("AnuladorNombre")) ? null : new Usuario { Nombre = reader.GetString("AnuladorNombre")!, Apellido = reader.GetString("AnuladorApellido")! }
                            };
                        }
                    }
                }
            }
            return pago;
        }

        public void Create(Pago pago)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Pagos (NumeroPago, ContratoId, FechaPago, Importe, Detalle, Estado, UsuarioIdCreador) VALUES (@NumeroPago, @ContratoId, @FechaPago, @Importe, @Detalle, @Estado, @UsuarioIdCreador)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@NumeroPago", pago.NumeroPago);
                    command.Parameters.AddWithValue("@ContratoId", pago.ContratoId);
                    command.Parameters.AddWithValue("@FechaPago", pago.FechaPago);
                    command.Parameters.AddWithValue("@Importe", pago.Importe);
                    command.Parameters.AddWithValue("@Detalle", (object)pago.Detalle ?? DBNull.Value);
                    command.Parameters.AddWithValue("@Estado", "Vigente");
                    command.Parameters.AddWithValue("@UsuarioIdCreador", pago.UsuarioIdCreador);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Anular(int id, int usuarioId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sqlUpdate = "UPDATE Pagos SET Estado = @Estado, UsuarioIdAnulador = @UsuarioIdAnulador WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlUpdate, connection))
                {
                    command.Parameters.AddWithValue("@Estado", "Anulado");
                    command.Parameters.AddWithValue("@UsuarioIdAnulador", usuarioId);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }
        
        public int GetContratoIdByPagoId(int id)
        {
            int contratoId = 0;
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sqlSelect = "SELECT ContratoId FROM Pagos WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlSelect, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    var result = command.ExecuteScalar();
                    if (result != null) contratoId = Convert.ToInt32(result);
                }
            }
            return contratoId;
        }
    }
}