using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InmobiliariaWebApp.Migrations
{
    /// <inheritdoc />
    public partial class ContratoTerminacionMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRescision",
                table: "Contratos",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Multa",
                table: "Contratos",
                type: "decimal(65,30)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaRescision",
                table: "Contratos");

            migrationBuilder.DropColumn(
                name: "Multa",
                table: "Contratos");
        }
    }
}
