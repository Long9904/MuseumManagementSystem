using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MuseumSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameVisitorIdToId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "Visitors",
                newName: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Visitors",
                newName: "VisitorId");
        }
    }
}
