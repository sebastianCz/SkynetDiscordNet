using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skynet.Migrations
{
    /// <inheritdoc />
    public partial class updatedSearchResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PlaylistName",
                table: "Results",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "SongName",
                table: "Results",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SongUri",
                table: "Results",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SongName",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "SongUri",
                table: "Results");

            migrationBuilder.AlterColumn<string>(
                name: "PlaylistName",
                table: "Results",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
