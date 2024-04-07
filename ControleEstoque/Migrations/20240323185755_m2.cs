using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ControleEstoque.Migrations
{
    /// <inheritdoc />
    public partial class m2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FrenteServicoId",
                table: "Movimentacoes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FrenteServicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nome = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FrenteServicos", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Movimentacoes_FrenteServicoId",
                table: "Movimentacoes",
                column: "FrenteServicoId");

            migrationBuilder.AddForeignKey(
                name: "FK_Movimentacoes_FrenteServicos_FrenteServicoId",
                table: "Movimentacoes",
                column: "FrenteServicoId",
                principalTable: "FrenteServicos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Movimentacoes_FrenteServicos_FrenteServicoId",
                table: "Movimentacoes");

            migrationBuilder.DropTable(
                name: "FrenteServicos");

            migrationBuilder.DropIndex(
                name: "IX_Movimentacoes_FrenteServicoId",
                table: "Movimentacoes");

            migrationBuilder.DropColumn(
                name: "FrenteServicoId",
                table: "Movimentacoes");
        }
    }
}
