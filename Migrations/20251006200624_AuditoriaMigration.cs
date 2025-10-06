using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InmobiliariaWebApp.Migrations
{
    /// <inheritdoc />
    public partial class AuditoriaMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdAnulador",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdCreador",
                table: "Pagos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdCreador",
                table: "Contratos",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioIdTerminador",
                table: "Contratos",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsuarioIdAnulador",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioIdCreador",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UsuarioIdCreador",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "UsuarioIdTerminador",
                table: "Contratos");
        }
    }
}
