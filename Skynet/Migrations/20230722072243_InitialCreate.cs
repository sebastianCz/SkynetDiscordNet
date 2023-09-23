using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Skynet.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GuildMusicDatas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AutoplayOn = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMusicDatas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUsed = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MusicPlaylist",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuildMusicDataId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicPlaylist", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicPlaylist_GuildMusicDatas_GuildMusicDataId",
                        column: x => x.GuildMusicDataId,
                        principalTable: "GuildMusicDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Probabilities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildMusicDataId = table.Column<int>(type: "int", nullable: false),
                    AutoplayGuildPlaylists = table.Column<int>(type: "int", nullable: false),
                    AutoPlayUserTerms = table.Column<int>(type: "int", nullable: false),
                    AutoplayRandomTerm = table.Column<int>(type: "int", nullable: false),
                    AutoPlayRandomPlaylist = table.Column<int>(type: "int", nullable: false),
                    AutoPlayDefaultTracks = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Probabilities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Probabilities_GuildMusicDatas_GuildMusicDataId",
                        column: x => x.GuildMusicDataId,
                        principalTable: "GuildMusicDatas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MusicSearchTerm",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AddedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Term = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GuildMusicDataId = table.Column<int>(type: "int", nullable: true),
                    MusicUserId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MusicSearchTerm", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MusicSearchTerm_GuildMusicDatas_GuildMusicDataId",
                        column: x => x.GuildMusicDataId,
                        principalTable: "GuildMusicDatas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_MusicSearchTerm_MusicUsers_MusicUserId",
                        column: x => x.MusicUserId,
                        principalTable: "MusicUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LavalinkTrackBot",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GuildMusicDataId = table.Column<int>(type: "int", nullable: true),
                    MusicPlaylistId = table.Column<int>(type: "int", nullable: true),
                    Author = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Uri = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LavalinkTrackBot", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LavalinkTrackBot_GuildMusicDatas_GuildMusicDataId",
                        column: x => x.GuildMusicDataId,
                        principalTable: "GuildMusicDatas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LavalinkTrackBot_MusicPlaylist_MusicPlaylistId",
                        column: x => x.MusicPlaylistId,
                        principalTable: "MusicPlaylist",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_LavalinkTrackBot_GuildMusicDataId",
                table: "LavalinkTrackBot",
                column: "GuildMusicDataId");

            migrationBuilder.CreateIndex(
                name: "IX_LavalinkTrackBot_MusicPlaylistId",
                table: "LavalinkTrackBot",
                column: "MusicPlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicPlaylist_GuildMusicDataId",
                table: "MusicPlaylist",
                column: "GuildMusicDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicSearchTerm_GuildMusicDataId",
                table: "MusicSearchTerm",
                column: "GuildMusicDataId");

            migrationBuilder.CreateIndex(
                name: "IX_MusicSearchTerm_MusicUserId",
                table: "MusicSearchTerm",
                column: "MusicUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Probabilities_GuildMusicDataId",
                table: "Probabilities",
                column: "GuildMusicDataId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LavalinkTrackBot");

            migrationBuilder.DropTable(
                name: "MusicSearchTerm");

            migrationBuilder.DropTable(
                name: "Probabilities");

            migrationBuilder.DropTable(
                name: "MusicPlaylist");

            migrationBuilder.DropTable(
                name: "MusicUsers");

            migrationBuilder.DropTable(
                name: "GuildMusicDatas");
        }
    }
}
