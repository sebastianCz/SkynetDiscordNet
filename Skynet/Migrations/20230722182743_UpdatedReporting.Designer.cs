﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Skynet.db;

#nullable disable

namespace Skynet.Migrations
{
    [DbContext(typeof(BotContext))]
    [Migration("20230722182743_UpdatedReporting")]
    partial class UpdatedReporting
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.9")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Skynet.Domain.GuildData.GuildMusicData", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("AutoplayOn")
                        .HasColumnType("bit");

                    b.Property<string>("DiscordId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("GuildMusicDatas");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.LavalinkTrackBot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Author")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("GuildMusicDataId")
                        .HasColumnType("int");

                    b.Property<int?>("MusicPlaylistId")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Uri")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GuildMusicDataId");

                    b.HasIndex("MusicPlaylistId");

                    b.ToTable("LavalinkTrackBot");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicPlaylist", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GuildMusicDataId")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GuildMusicDataId");

                    b.ToTable("MusicPlaylist");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicSearchTerm", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("AddedOn")
                        .HasColumnType("datetime2");

                    b.Property<int?>("GuildMusicDataId")
                        .HasColumnType("int");

                    b.Property<int?>("MusicUserId")
                        .HasColumnType("int");

                    b.Property<string>("Term")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("GuildMusicDataId");

                    b.HasIndex("MusicUserId");

                    b.ToTable("MusicSearchTerm");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("LastUsed")
                        .HasColumnType("datetime2");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("MusicUsers");
                });

            modelBuilder.Entity("Skynet.Domain.SearchEngineResult", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("GuildMusicDataId")
                        .HasColumnType("int");

                    b.Property<int>("LavalinkSearchType")
                        .HasColumnType("int");

                    b.Property<string>("PlaylistName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PlaylistReceived")
                        .HasColumnType("bit");

                    b.Property<int>("SearchInput")
                        .HasColumnType("int");

                    b.Property<int>("SearchType")
                        .HasColumnType("int");

                    b.Property<string>("SongAuthor")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SongName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SongUri")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("Successfull")
                        .HasColumnType("bit");

                    b.HasKey("Id");

                    b.HasIndex("GuildMusicDataId");

                    b.ToTable("Results");
                });

            modelBuilder.Entity("Skynet.Domain.SearchProbability", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int>("AutoPlayDefaultTracks")
                        .HasColumnType("int");

                    b.Property<int>("AutoPlayRandomPlaylist")
                        .HasColumnType("int");

                    b.Property<int>("AutoPlayUserTerms")
                        .HasColumnType("int");

                    b.Property<int>("AutoplayGuildPlaylists")
                        .HasColumnType("int");

                    b.Property<int>("AutoplayRandomTerm")
                        .HasColumnType("int");

                    b.Property<int>("GuildMusicDataId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("GuildMusicDataId")
                        .IsUnique();

                    b.ToTable("Probabilities");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.LavalinkTrackBot", b =>
                {
                    b.HasOne("Skynet.Domain.GuildData.GuildMusicData", "GuildMusicData")
                        .WithMany("ActivePlaylist")
                        .HasForeignKey("GuildMusicDataId");

                    b.HasOne("Skynet.Domain.GuildData.MusicPlaylist", "MusicPlaylist")
                        .WithMany("Playlist")
                        .HasForeignKey("MusicPlaylistId");

                    b.Navigation("GuildMusicData");

                    b.Navigation("MusicPlaylist");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicPlaylist", b =>
                {
                    b.HasOne("Skynet.Domain.GuildData.GuildMusicData", "GuildMusicData")
                        .WithMany("Playlists")
                        .HasForeignKey("GuildMusicDataId");

                    b.Navigation("GuildMusicData");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicSearchTerm", b =>
                {
                    b.HasOne("Skynet.Domain.GuildData.GuildMusicData", "GuildMusicData")
                        .WithMany("SearchTerms")
                        .HasForeignKey("GuildMusicDataId");

                    b.HasOne("Skynet.Domain.GuildData.MusicUser", "MusicUser")
                        .WithMany("SearchTerms")
                        .HasForeignKey("MusicUserId");

                    b.Navigation("GuildMusicData");

                    b.Navigation("MusicUser");
                });

            modelBuilder.Entity("Skynet.Domain.SearchEngineResult", b =>
                {
                    b.HasOne("Skynet.Domain.GuildData.GuildMusicData", "GuildMusicData")
                        .WithMany("SearchEngines")
                        .HasForeignKey("GuildMusicDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildMusicData");
                });

            modelBuilder.Entity("Skynet.Domain.SearchProbability", b =>
                {
                    b.HasOne("Skynet.Domain.GuildData.GuildMusicData", "GuildMusicData")
                        .WithOne("SearchProbability")
                        .HasForeignKey("Skynet.Domain.SearchProbability", "GuildMusicDataId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GuildMusicData");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.GuildMusicData", b =>
                {
                    b.Navigation("ActivePlaylist");

                    b.Navigation("Playlists");

                    b.Navigation("SearchEngines");

                    b.Navigation("SearchProbability")
                        .IsRequired();

                    b.Navigation("SearchTerms");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicPlaylist", b =>
                {
                    b.Navigation("Playlist");
                });

            modelBuilder.Entity("Skynet.Domain.GuildData.MusicUser", b =>
                {
                    b.Navigation("SearchTerms");
                });
#pragma warning restore 612, 618
        }
    }
}
