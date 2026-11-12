using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AdicionadoStatusAoChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "emailLogin",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "nomeUsuario",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "senhaLogin",
                table: "Configuracoes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "emailLogin",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "nomeUsuario",
                table: "Configuracoes");

            migrationBuilder.DropColumn(
                name: "senhaLogin",
                table: "Configuracoes");
        }
    }
}
