using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skynet.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedReporting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LavalinkSearchType",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LavalinkSearchType",
                table: "Results");
        }
    }
}
