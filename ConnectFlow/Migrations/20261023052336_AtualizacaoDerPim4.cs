using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoDerPim4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdUsuario",
                table: "Configuracoes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ChamadoSuporte",
                columns: table => new
                {
                    IdChamado = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    NomeCompleto = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailCorporativo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DataEnvio = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChamadoSuporte", x => x.IdChamado);
                    table.ForeignKey(
                        name: "FK_ChamadoSuporte_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SenhaHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Perfil = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DataCriacao = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configuracoes_IdUsuario",
                table: "Configuracoes",
                column: "IdUsuario");

            migrationBuilder.CreateIndex(
                name: "IX_ChamadoSuporte_IdCliente",
                table: "ChamadoSuporte",
                column: "IdCliente");

            migrationBuilder.AddForeignKey(
                name: "FK_Configuracoes_Usuarios_IdUsuario",
                table: "Configuracoes",
                column: "IdUsuario",
                principalTable: "Usuarios",
                principalColumn: "IdUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Configuracoes_Usuarios_IdUsuario",
                table: "Configuracoes");

            migrationBuilder.DropTable(
                name: "ChamadoSuporte");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_Configuracoes_IdUsuario",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "IdUsuario",
                table: "Configuracoes");
        }
    }
}
