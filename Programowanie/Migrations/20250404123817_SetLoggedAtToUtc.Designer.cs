﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FitnessApp.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20250404123817_SetLoggedAtToUtc")]
    partial class SetLoggedAtToUtc
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FitnessApp.Models.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Brands")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("EnergyValue")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Products");
                });

            modelBuilder.Entity("FitnessApp.Models.ProductLogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Brands")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Energy")
                        .HasColumnType("double precision");

                    b.Property<string>("EnergyUnit")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Fat")
                        .HasColumnType("double precision");

                    b.Property<double>("Grams")
                        .HasColumnType("double precision");

                    b.Property<DateTime>("LoggedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("ProductName")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<double>("Proteins")
                        .HasColumnType("double precision");

                    b.Property<double>("Salt")
                        .HasColumnType("double precision");

                    b.Property<double>("Sugars")
                        .HasColumnType("double precision");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("ProductLogEntries");
                });

            modelBuilder.Entity("FitnessApp.Models.Product", b =>
                {
                    b.OwnsOne("FitnessApp.Models.Nutriments", "Nutriments", b1 =>
                        {
                            b1.Property<int>("ProductId")
                                .HasColumnType("integer");

                            b1.Property<double>("Energy")
                                .HasColumnType("double precision");

                            b1.Property<string>("EnergyUnit")
                                .IsRequired()
                                .HasColumnType("text");

                            b1.Property<double>("Fat")
                                .HasColumnType("double precision");

                            b1.Property<double>("Proteins")
                                .HasColumnType("double precision");

                            b1.Property<double>("Salt")
                                .HasColumnType("double precision");

                            b1.Property<double>("Sugars")
                                .HasColumnType("double precision");

                            b1.HasKey("ProductId");

                            b1.ToTable("Products");

                            b1.WithOwner()
                                .HasForeignKey("ProductId");
                        });

                    b.Navigation("Nutriments")
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
