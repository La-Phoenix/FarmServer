﻿// <auto-generated />
using System;
using FarmServer.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FarmServer.Migrations
{
    [DbContext(typeof(FarmDbContext))]
    partial class FarmDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FarmFarmer", b =>
                {
                    b.Property<Guid>("FarmersId")
                        .HasColumnType("uuid");

                    b.Property<Guid>("FarmsId")
                        .HasColumnType("uuid");

                    b.HasKey("FarmersId", "FarmsId");

                    b.HasIndex("FarmsId");

                    b.ToTable("FarmFarmer");
                });

            modelBuilder.Entity("FarmServer.Domain.Entities.Farm", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Farms");
                });

            modelBuilder.Entity("FarmServer.Domain.Entities.Farmer", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Location")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Farmers");
                });

            modelBuilder.Entity("FarmServer.Domain.Entities.Field", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<decimal>("Area")
                        .HasColumnType("numeric");

                    b.Property<string>("CropType")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<Guid>("FarmId")
                        .HasColumnType("uuid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("FarmId");

                    b.ToTable("Fields");
                });

            modelBuilder.Entity("FarmFarmer", b =>
                {
                    b.HasOne("FarmServer.Domain.Entities.Farmer", null)
                        .WithMany()
                        .HasForeignKey("FarmersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FarmServer.Domain.Entities.Farm", null)
                        .WithMany()
                        .HasForeignKey("FarmsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("FarmServer.Domain.Entities.Field", b =>
                {
                    b.HasOne("FarmServer.Domain.Entities.Farm", "Farm")
                        .WithMany("Fields")
                        .HasForeignKey("FarmId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Farm");
                });

            modelBuilder.Entity("FarmServer.Domain.Entities.Farm", b =>
                {
                    b.Navigation("Fields");
                });
#pragma warning restore 612, 618
        }
    }
}
