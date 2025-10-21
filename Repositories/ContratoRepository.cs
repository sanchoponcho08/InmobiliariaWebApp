using InmobiliariaWebApp.Models;
using MySqlConnector;
using System.Data;

namespace InmobiliariaWebApp.Repositories
{
    public class ContratoRepository : RepositoryBase, IContratoRepository
    {
        public ContratoRepository(IConfiguration configuration) : base(configuration)
        {
        }

        public List<Contrato> GetAll()
        {
            var contratos = new List<Contrato>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, c.InquilinoId, c.InmuebleId,
                           i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                           im.Direccion AS InmuebleDireccion,
                           p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido
                    FROM Contratos c
                    LEFT JOIN Inquilinos i ON c.InquilinoId = i.Id
                    LEFT JOIN Inmuebles im ON c.InmuebleId = im.Id
                    LEFT JOIN Propietarios p ON im.PropietarioId = p.Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            contratos.Add(new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                Inquilino = new Inquilino
                                {
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("InquilinoNombre")) ? "" : reader.GetString("InquilinoNombre")!,
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("InquilinoApellido")) ? "" : reader.GetString("InquilinoApellido")!
                                },
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.IsDBNull(reader.GetOrdinal("InmuebleDireccion")) ? "" : reader.GetString("InmuebleDireccion")!,
                                    Dueño = new Propietario
                                    {
                                        Nombre = reader.IsDBNull(reader.GetOrdinal("PropietarioNombre")) ? "" : reader.GetString("PropietarioNombre")!,
                                        Apellido = reader.IsDBNull(reader.GetOrdinal("PropietarioApellido")) ? "" : reader.GetString("PropietarioApellido")!
                                    }
                                }
                            });
                        }
                    }
                }
            }
            return contratos;
        }

        public Contrato? GetById(int id)
        {
            Contrato? contrato = null;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = @"
                    SELECT c.Id, c.FechaInicio, c.FechaFin, c.MontoAlquiler, c.InquilinoId, c.InmuebleId, c.FechaRescision, c.Multa,
                        i.Nombre AS InquilinoNombre, i.Apellido AS InquilinoApellido,
                        im.Direccion AS InmuebleDireccion,
                        p.Nombre AS PropietarioNombre, p.Apellido AS PropietarioApellido,
                        uc.Nombre AS CreadorNombre, uc.Apellido AS CreadorApellido,
                        ut.Nombre AS TerminadorNombre, ut.Apellido AS TerminadorApellido
                    FROM Contratos c
                    LEFT JOIN Inquilinos i ON c.InquilinoId = i.Id
                    LEFT JOIN Inmuebles im ON c.InmuebleId = im.Id
                    LEFT JOIN Propietarios p ON im.PropietarioId = p.Id
                    LEFT JOIN Usuarios uc ON c.UsuarioIdCreador = uc.Id
                    LEFT JOIN Usuarios ut ON c.UsuarioIdTerminador = ut.Id
                    WHERE c.Id = @Id";

                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            contrato = new Contrato
                            {
                                Id = reader.GetInt32("Id"),
                                InquilinoId = reader.GetInt32("InquilinoId"),
                                InmuebleId = reader.GetInt32("InmuebleId"),
                                FechaInicio = reader.GetDateTime("FechaInicio"),
                                FechaFin = reader.GetDateTime("FechaFin"),
                                MontoAlquiler = reader.GetDecimal("MontoAlquiler"),
                                FechaRescision = reader.IsDBNull(reader.GetOrdinal("FechaRescision")) ? (DateTime?)null : reader.GetDateTime("FechaRescision"),
                                Multa = reader.IsDBNull(reader.GetOrdinal("Multa")) ? (decimal?)null : reader.GetDecimal("Multa"),
                                Inquilino = new Inquilino { 
                                    Nombre = reader.IsDBNull(reader.GetOrdinal("InquilinoNombre")) ? "" : reader.GetString("InquilinoNombre"), 
                                    Apellido = reader.IsDBNull(reader.GetOrdinal("InquilinoApellido")) ? "" : reader.GetString("InquilinoApellido") 
                                },
                                Inmueble = new Inmueble
                                {
                                    Direccion = reader.IsDBNull(reader.GetOrdinal("InmuebleDireccion")) ? "" : reader.GetString("InmuebleDireccion"),
                                    Dueño = new Propietario { 
                                        Nombre = reader.IsDBNull(reader.GetOrdinal("PropietarioNombre")) ? "" : reader.GetString("PropietarioNombre"), 
                                        Apellido = reader.IsDBNull(reader.GetOrdinal("PropietarioApellido")) ? "" : reader.GetString("PropietarioApellido")
                                    }
                                },
                                Creador = reader.IsDBNull(reader.GetOrdinal("CreadorNombre")) ? null : new Usuario { 
                                    Nombre = reader.GetString("CreadorNombre"), 
                                    Apellido = reader.GetString("CreadorApellido") 
                                },
                                Terminador = reader.IsDBNull(reader.GetOrdinal("TerminadorNombre")) ? null : new Usuario { 
                                    Nombre = reader.GetString("TerminadorNombre"), 
                                    Apellido = reader.GetString("TerminadorApellido")
                                }
                            };
                        }
                    }
                }
            }
            return contrato;
        }

        public void Create(Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "INSERT INTO Contratos (InquilinoId, InmuebleId, FechaInicio, FechaFin, MontoAlquiler, UsuarioIdCreador, FechaRescision, Multa) VALUES (@InquilinoId, @InmuebleId, @FechaInicio, @FechaFin, @MontoAlquiler, @UsuarioIdCreador, NULL, 0)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                    command.Parameters.AddWithValue("@MontoAlquiler", contrato.MontoAlquiler);
                    command.Parameters.AddWithValue("@UsuarioIdCreador", contrato.UsuarioIdCreador);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Update(int id, Contrato contrato)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "UPDATE Contratos SET InquilinoId = @InquilinoId, InmuebleId = @InmuebleId, FechaInicio = @FechaInicio, FechaFin = @FechaFin, MontoAlquiler = @MontoAlquiler WHERE Id = @Id";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InquilinoId", contrato.InquilinoId);
                    command.Parameters.AddWithValue("@InmuebleId", contrato.InmuebleId);
                    command.Parameters.AddWithValue("@FechaInicio", contrato.FechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", contrato.FechaFin);
                    command.Parameters.AddWithValue("@MontoAlquiler", contrato.MontoAlquiler);
                    command.Parameters.AddWithValue("@Id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sqlPagos = "DELETE FROM Pagos WHERE ContratoId = @ContratoId";
                using (var command = new MySqlCommand(sqlPagos, connection))
                {
                    command.Parameters.AddWithValue("@ContratoId", id);
                    command.ExecuteNonQuery();
                }
                string sqlContrato = "DELETE FROM Contratos WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlContrato, connection))
                {
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }
            }
        }

        public void TerminarContrato(int id, decimal multa, int usuarioId)
        {
            using (var connection = new MySqlConnection(connectionString))
            {
                connection.Open();
                string sqlContrato = "UPDATE Contratos SET FechaRescision = @FechaRescision, Multa = @Multa, UsuarioIdTerminador = @UsuarioIdTerminador WHERE Id = @Id";
                using (var command = new MySqlCommand(sqlContrato, connection))
                {
                    command.Parameters.AddWithValue("@FechaRescision", DateTime.Now);
                    command.Parameters.AddWithValue("@Multa", multa);
                    command.Parameters.AddWithValue("@UsuarioIdTerminador", usuarioId);
                    command.Parameters.AddWithValue("@Id", id);
                    command.ExecuteNonQuery();
                }

                string sqlPago = "INSERT INTO Pagos (NumeroPago, ContratoId, FechaPago, Importe, Detalle, Estado, UsuarioIdCreador) VALUES (@NumeroPago, @ContratoId, @FechaPago, @Importe, @Detalle, @Estado, @UsuarioIdCreador)";
                using (var command = new MySqlCommand(sqlPago, connection))
                {
                    command.Parameters.AddWithValue("@NumeroPago", 99);
                    command.Parameters.AddWithValue("@ContratoId", id);
                    command.Parameters.AddWithValue("@FechaPago", DateTime.Now);
                    command.Parameters.AddWithValue("@Importe", multa);
                    command.Parameters.AddWithValue("@Detalle", "Pago de multa por rescisión anticipada");
                    command.Parameters.AddWithValue("@Estado", "Vigente");
                    command.Parameters.AddWithValue("@UsuarioIdCreador", usuarioId);
                    command.ExecuteNonQuery();
                }
            }
        }

        public bool VerificarSuperposicion(int inmuebleId, DateTime fechaInicio, DateTime fechaFin, int? contratoId = null)
        {
            bool seSuperpone = false;
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT COUNT(*) FROM Contratos WHERE InmuebleId = @InmuebleId AND @FechaInicio < FechaFin AND @FechaFin > FechaInicio AND (@ContratoId IS NULL OR Id != @ContratoId)";
                using (var command = new MySqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@InmuebleId", inmuebleId);
                    command.Parameters.AddWithValue("@FechaInicio", fechaInicio);
                    command.Parameters.AddWithValue("@FechaFin", fechaFin);
                    command.Parameters.AddWithValue("@ContratoId", contratoId.HasValue ? (object)contratoId.Value : DBNull.Value);

                    connection.Open();
                    long count = Convert.ToInt64(command.ExecuteScalar());
                    seSuperpone = count > 0;
                }
            }
            return seSuperpone;
        }

        public List<Inquilino> GetInquilinos()
        {
            var inquilinos = new List<Inquilino>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Nombre, Apellido FROM Inquilinos";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inquilinos.Add(new Inquilino { Id = reader.GetInt32("Id"), Nombre = reader.GetString("Nombre")!, Apellido = reader.GetString("Apellido")! });
                        }
                    }
                }
            }
            return inquilinos;
        }

        public List<Inmueble> GetInmuebles()
        {
            var inmuebles = new List<Inmueble>();
            using (var connection = new MySqlConnection(connectionString))
            {
                string sql = "SELECT Id, Direccion FROM Inmuebles";
                using (var command = new MySqlCommand(sql, connection))
                {
                    connection.Open();
                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            inmuebles.Add(new Inmueble { Id = reader.GetInt32("Id"), Direccion = reader.GetString("Direccion")! });
                        }
                    }
                }
            }
            return inmuebles;
        }
    }
}
