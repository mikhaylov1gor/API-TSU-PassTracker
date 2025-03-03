﻿// <auto-generated />
using System;
using API_TSU_PassTracker.Models.DB;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace API_TSU_PassTracker.Migrations
{
    [DbContext(typeof(TsuPassTrackerDBContext))]
    [Migration("20250303183931_RequestCreatedDate")]
    partial class RequestCreatedDate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Confirmation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<int>("ConfirmationType")
                        .HasColumnType("integer");

                    b.Property<Guid>("RequestId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("RequestId")
                        .IsUnique();

                    b.ToTable("Confirmation");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.ConfirmationFile", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<Guid>("ConfirmationId")
                        .HasColumnType("uuid");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("bytea");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("ConfirmationId");

                    b.ToTable("ConfirmationFile");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Request", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateFrom")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("DateTo")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("Status")
                        .HasColumnType("integer");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Request");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.TokenBlackList", b =>
                {
                    b.Property<Guid>("id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<DateTime>("expirationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("id");

                    b.ToTable("TokenBlackList");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.PrimitiveCollection<int[]>("Roles")
                        .IsRequired()
                        .HasColumnType("integer[]");

                    b.Property<string>("Salt")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("User");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Confirmation", b =>
                {
                    b.HasOne("API_TSU_PassTracker.Models.DB.Request", null)
                        .WithOne("Confirmation")
                        .HasForeignKey("API_TSU_PassTracker.Models.DB.Confirmation", "RequestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.ConfirmationFile", b =>
                {
                    b.HasOne("API_TSU_PassTracker.Models.DB.Confirmation", "Confirmation")
                        .WithMany("Files")
                        .HasForeignKey("ConfirmationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Confirmation");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Request", b =>
                {
                    b.HasOne("API_TSU_PassTracker.Models.DB.User", "User")
                        .WithMany("Requests")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Confirmation", b =>
                {
                    b.Navigation("Files");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.Request", b =>
                {
                    b.Navigation("Confirmation");
                });

            modelBuilder.Entity("API_TSU_PassTracker.Models.DB.User", b =>
                {
                    b.Navigation("Requests");
                });
#pragma warning restore 612, 618
        }
    }
}
