﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PeepoGuessrApi.Databases;

#nullable disable

namespace PeepoGuessrApi.Migrations.LobbyDb
{
    [DbContext(typeof(LobbyDbContext))]
    [Migration("20230707140330_LobbyDbV0.1")]
    partial class LobbyDbV01
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.LobbyType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("LobbyTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Name = "Singleplayer"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Multiplayer"
                        },
                        new
                        {
                            Id = 3,
                            Name = "PartyBattle"
                        },
                        new
                        {
                            Id = 4,
                            Name = "RandomEvents"
                        });
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DivisionId")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsGameFounded")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRandomAcceptable")
                        .HasColumnType("boolean");

                    b.Property<int>("LobbyTypeId")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Score")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("LobbyTypeId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.UserInvite", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("RequestedId")
                        .HasColumnType("integer");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserInvites");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.User", b =>
                {
                    b.HasOne("PeepoGuessrApi.Entities.Databases.Lobby.LobbyType", "LobbyType")
                        .WithMany("Users")
                        .HasForeignKey("LobbyTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("LobbyType");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.UserInvite", b =>
                {
                    b.HasOne("PeepoGuessrApi.Entities.Databases.Lobby.User", "User")
                        .WithMany("UserInvites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.LobbyType", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Lobby.User", b =>
                {
                    b.Navigation("UserInvites");
                });
#pragma warning restore 612, 618
        }
    }
}