using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skynet.Migrations
{
    /// <inheritdoc />
    public partial class updatedSearch : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildMusicDataId = table.Column<int>(type: "int", nullable: false),
                    PlaylistReceived = table.Column<bool>(type: "bit", nullable: false),
                    PlaylistName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SearchInput = table.Column<int>(type: "int", nullable: false),
                    SearchType = table.Column<int>(type: "int", nullable: false),
                    Successfull = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_GuildMusicDatas_GuildMusicDataId",
                        column: x => x.GuildMusicDataId,
                        principalTable: "GuildMusicDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_GuildMusicDataId",
                table: "Results",
                column: "GuildMusicDataId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");
        }
    }
}
