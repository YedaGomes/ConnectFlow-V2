using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConnectFlow.Migrations
{
    /// <inheritdoc />
    public partial class AdicionarColunaStatusChamado : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "ChamadoSuporte",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChamadoSuporte");
        }
    }
}
