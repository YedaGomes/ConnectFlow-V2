using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AtualizacaoDoModelo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cliente",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Cnpj = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    RazaoSocial = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NomeFantasia = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EmailCorporativo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefone = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    ResponsavelTecnico = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SistemasOrigem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SistemasDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolumeDadosMensal = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuporteLibras = table.Column<bool>(type: "bit", nullable: false),
                    NotificacoesEmail = table.Column<bool>(type: "bit", nullable: false),
                    DataCadastro = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cliente", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Configuracoes",
                columns: table => new
                {
                    IdConfig = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCliente = table.Column<int>(type: "int", nullable: false),
                    SistemaOrigem = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SistemaDestino = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VolumeDados = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SuporteLibras = table.Column<bool>(type: "bit", nullable: false),
                    NotificacoesEmail = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuracoes", x => x.IdConfig);
                    table.ForeignKey(
                        name: "FK_Configuracoes_Cliente_IdCliente",
                        column: x => x.IdCliente,
                        principalTable: "Cliente",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    IdLog = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdConfig = table.Column<int>(type: "int", nullable: false),
                    DataHora = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Mensagem = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.IdLog);
                    table.ForeignKey(
                        name: "FK_Logs_Configuracoes_IdConfig",
                        column: x => x.IdConfig,
                        principalTable: "Configuracoes",
                        principalColumn: "IdConfig",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Configuracoes_IdCliente",
                table: "Configuracoes",
                column: "IdCliente");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_IdConfig",
                table: "Logs",
                column: "IdConfig");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");

            migrationBuilder.DropTable(
                name: "Configuracoes");

            migrationBuilder.DropTable(
                name: "Cliente");
        }
    }
}
