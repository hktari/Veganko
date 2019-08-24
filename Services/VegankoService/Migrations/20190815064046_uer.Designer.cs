﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using VegankoService.Data;

namespace VegankoService.Migrations
{
    [DbContext(typeof(VegankoContext))]
    [Migration("20190815064046_uer")]
    partial class uer
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.11-servicing-32099")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("VegankoService.Models.Comment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ProductId");

                    b.Property<int>("Rating");

                    b.Property<string>("Text");

                    b.Property<string>("UserId");

                    b.Property<DateTime>("UtcDatePosted");

                    b.HasKey("Id");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("VegankoService.Models.Product", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Barcode");

                    b.Property<string>("Brand");

                    b.Property<string>("Description");

                    b.Property<string>("ImageBase64Encoded");

                    b.Property<string>("ImageName");

                    b.Property<string>("Name");

                    b.Property<int>("ProductClassifiers");

                    b.Property<int>("Rating");

                    b.Property<int>("State");

                    b.Property<string>("Type");

                    b.HasKey("Id");

                    b.ToTable("Product");
                });

            modelBuilder.Entity("VegankoService.Models.User.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<int>("AccessRights");

                    b.Property<string>("AvatarId");

                    b.Property<string>("ConcurrencyStamp");

                    b.Property<string>("Description");

                    b.Property<string>("Email");

                    b.Property<bool>("EmailConfirmed");

                    b.Property<string>("Label");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail");

                    b.Property<string>("NormalizedUserName");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<string>("ProfileBackgroundId");

                    b.Property<string>("SecurityStamp");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName");

                    b.HasKey("Id");

                    b.ToTable("ApplicationUser");
                });
#pragma warning restore 612, 618
        }
    }
}