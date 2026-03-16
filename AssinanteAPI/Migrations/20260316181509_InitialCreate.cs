using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Dando preferência a matrizes irregulares do que matrizes multidimensionais

namespace AssinanteAPI.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assinantes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DataInicioAssinatura = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Plano = table.Column<int>(type: "int", nullable: false),
                    ValorMensal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false, defaultValue: 1)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assinantes", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Assinantes",
                columns: new[] { "Id", "DataInicioAssinatura", "Email", "NomeCompleto", "Plano", "Status", "ValorMensal" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "joao.silva@email.com", "João Silva", 1, 1, 29.90m },
                    { 2, new DateTime(2023, 6, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "maria.santos@email.com", "Maria Santos", 2, 1, 49.90m },
                    { 3, new DateTime(2022, 12, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "pedro.costa@email.com", "Pedro Costa", 3, 1, 99.90m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assinantes_Email",
                table: "Assinantes",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assinantes");
        }
    }
}
