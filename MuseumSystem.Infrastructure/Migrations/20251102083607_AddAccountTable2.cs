using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseumSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAccountTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Visitors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "HistoricalContexts",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExhibitionId",
                table: "HistoricalContexts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExhibitionId",
                table: "Artifacts",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_HistoricalContexts_ExhibitionId",
                table: "HistoricalContexts",
                column: "ExhibitionId");

            migrationBuilder.CreateIndex(
                name: "IX_Artifacts_ExhibitionId",
                table: "Artifacts",
                column: "ExhibitionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Artifacts_Exhibitions_ExhibitionId",
                table: "Artifacts",
                column: "ExhibitionId",
                principalTable: "Exhibitions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_HistoricalContexts_Exhibitions_ExhibitionId",
                table: "HistoricalContexts",
                column: "ExhibitionId",
                principalTable: "Exhibitions",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Artifacts_Exhibitions_ExhibitionId",
                table: "Artifacts");

            migrationBuilder.DropForeignKey(
                name: "FK_HistoricalContexts_Exhibitions_ExhibitionId",
                table: "HistoricalContexts");

            migrationBuilder.DropIndex(
                name: "IX_HistoricalContexts_ExhibitionId",
                table: "HistoricalContexts");

            migrationBuilder.DropIndex(
                name: "IX_Artifacts_ExhibitionId",
                table: "Artifacts");

            migrationBuilder.DropColumn(
                name: "ExhibitionId",
                table: "HistoricalContexts");

            migrationBuilder.DropColumn(
                name: "ExhibitionId",
                table: "Artifacts");

            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Visitors",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "HistoricalContexts",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
