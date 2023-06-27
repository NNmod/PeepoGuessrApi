﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using PeepoGuessrApi.Databases;

#nullable disable

namespace PeepoGuessrApi.Migrations.GameDb
{
    [DbContext(typeof(GameDbContext))]
    [Migration("20230627223106_GameDbV0.3")]
    partial class GameDbV03
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.Game", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<int>("GameTypeId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsRoundPromoted")
                        .HasColumnType("boolean");

                    b.Property<string>("MapUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Multiplier")
                        .HasColumnType("double precision");

                    b.Property<double>("PosX")
                        .HasColumnType("double precision");

                    b.Property<double>("PosY")
                        .HasColumnType("double precision");

                    b.Property<int>("RoundCount")
                        .HasColumnType("integer");

                    b.Property<DateTime>("RoundDelayExpire")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("RoundExpire")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("GameTypeId");

                    b.ToTable("Games");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.GameType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsPromotionEnable")
                        .HasColumnType("boolean");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("RoundDuration")
                        .HasColumnType("integer");

                    b.Property<int>("RoundPromotionDuration")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("GameTypes");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            IsPromotionEnable = true,
                            Name = "Singleplayer",
                            RoundDuration = 120,
                            RoundPromotionDuration = 5
                        },
                        new
                        {
                            Id = 2,
                            IsPromotionEnable = true,
                            Name = "Multiplayer",
                            RoundDuration = 300,
                            RoundPromotionDuration = 15
                        },
                        new
                        {
                            Id = 3,
                            IsPromotionEnable = false,
                            Name = "PartyBattle",
                            RoundDuration = 180,
                            RoundPromotionDuration = 0
                        },
                        new
                        {
                            Id = 4,
                            IsPromotionEnable = true,
                            Name = "RandomEvents",
                            RoundDuration = 300,
                            RoundPromotionDuration = 15
                        });
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("ConnectionId")
                        .HasColumnType("text");

                    b.Property<int>("DivisionId")
                        .HasColumnType("integer");

                    b.Property<int>("GameId")
                        .HasColumnType("integer");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("GameId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.Game", b =>
                {
                    b.HasOne("PeepoGuessrApi.Entities.Databases.Game.GameType", "GameType")
                        .WithMany("Games")
                        .HasForeignKey("GameTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GameType");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.User", b =>
                {
                    b.HasOne("PeepoGuessrApi.Entities.Databases.Game.Game", "Game")
                        .WithMany("Users")
                        .HasForeignKey("GameId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Game");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.Game", b =>
                {
                    b.Navigation("Users");
                });

            modelBuilder.Entity("PeepoGuessrApi.Entities.Databases.Game.GameType", b =>
                {
                    b.Navigation("Games");
                });
#pragma warning restore 612, 618
        }
    }
}
