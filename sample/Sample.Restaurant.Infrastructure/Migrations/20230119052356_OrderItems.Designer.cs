﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Sample.Restaurant.Infrastructure.Data;

#nullable disable

namespace Sample.Restaurant.Infrastructure.Migrations
{
    [DbContext(typeof(RestaurantDbContext))]
    [Migration("20230119052356_OrderItems")]
    partial class OrderItems
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasDefaultSchema("res")
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Lewee.Domain.DomainEventReference", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Dispatched")
                        .HasColumnType("bit");

                    b.Property<DateTime?>("DispatchedAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("DomainEventAssemblyName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("DomainEventClassName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("DomainEventJson")
                        .IsRequired()
                        .HasMaxLength(8000)
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("PersistedAt")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.HasIndex("Dispatched", "PersistedAt");

                    b.ToTable("DomainEventReferences", "res");
                });

            modelBuilder.Entity("Lewee.Domain.EnumEntity<Sample.Restaurant.Contracts.MenuItemType>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("MenuItemTypes", "res");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Food"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Drink"
                        });
                });

            modelBuilder.Entity("Lewee.Domain.EnumEntity<Sample.Restaurant.Contracts.OrderStatus>", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.HasKey("Id");

                    b.ToTable("OrderStatuses", "res");

                    b.HasData(
                        new
                        {
                            Id = 0,
                            Name = "Ordering"
                        },
                        new
                        {
                            Id = 1,
                            Name = "Order Placed"
                        },
                        new
                        {
                            Id = 2,
                            Name = "Updated"
                        },
                        new
                        {
                            Id = 3,
                            Name = "Paid"
                        });
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.MenuItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<int>("ItemTypeId")
                        .HasColumnType("int");

                    b.Property<DateTime>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)");

                    b.Property<decimal>("Price")
                        .HasPrecision(6, 2)
                        .HasColumnType("decimal(6,2)");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("ItemTypeId");

                    b.ToTable("MenuItems", "res");

                    b.HasData(
                        new
                        {
                            Id = new Guid("7fabe425-1d65-48d3-9ae4-caf5f27bbde8"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3229),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 0,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3232),
                            ModifiedBy = "System",
                            Name = "Pizza",
                            Price = 15m
                        },
                        new
                        {
                            Id = new Guid("70da3fcf-381d-4285-88e6-794b4b57e5b5"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3270),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 0,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3271),
                            ModifiedBy = "System",
                            Name = "Pasta",
                            Price = 15m
                        },
                        new
                        {
                            Id = new Guid("89669f56-63fe-4966-9028-f22f8d5a72f5"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3287),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 0,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3288),
                            ModifiedBy = "System",
                            Name = "Garlic Bread",
                            Price = 4.50m
                        },
                        new
                        {
                            Id = new Guid("1e1670cf-a80e-4421-a04f-6e28dc32a5d4"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3292),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 0,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3292),
                            ModifiedBy = "System",
                            Name = "Ice Cream",
                            Price = 5m
                        },
                        new
                        {
                            Id = new Guid("76495fb6-323e-4fbd-b0a4-5dbfcf61cef8"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3297),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 1,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3297),
                            ModifiedBy = "System",
                            Name = "Beer",
                            Price = 7.50m
                        },
                        new
                        {
                            Id = new Guid("58b91a73-682d-4696-b545-b493b56a0335"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3311),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 1,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3312),
                            ModifiedBy = "System",
                            Name = "Wine",
                            Price = 10m
                        },
                        new
                        {
                            Id = new Guid("110d16d7-3ce5-49dd-a187-d3640fdb42b5"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3323),
                            CreatedBy = "System",
                            IsDeleted = false,
                            ItemTypeId = 1,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 977, DateTimeKind.Utc).AddTicks(3324),
                            ModifiedBy = "System",
                            Name = "Soft Drink",
                            Price = 3.50m
                        });
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.Order", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("OrderStatusId")
                        .HasColumnType("int");

                    b.Property<Guid>("TableId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("OrderStatusId");

                    b.HasIndex("TableId");

                    b.ToTable("Orders", "res");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.OrderItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<Guid>("MenuItemId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<Guid>("OrderId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<decimal>("Price")
                        .HasPrecision(6, 2)
                        .HasColumnType("decimal(6,2)");

                    b.Property<int>("Quantity")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("MenuItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems", "res");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.Table", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("CreatedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsInUse")
                        .HasColumnType("bit");

                    b.Property<DateTime>("ModifiedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<string>("ModifiedBy")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)");

                    b.Property<int>("TableNumber")
                        .HasColumnType("int");

                    b.Property<byte[]>("Timestamp")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion");

                    b.HasKey("Id");

                    b.HasIndex("TableNumber")
                        .IsUnique();

                    b.ToTable("Tables", "res");

                    b.HasData(
                        new
                        {
                            Id = new Guid("dab4cd90-e6ca-48ec-b7a0-fcbe4e6e5805"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8450),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8452),
                            ModifiedBy = "System",
                            TableNumber = 1
                        },
                        new
                        {
                            Id = new Guid("7c181a4b-43a9-4d3c-acdc-dc00a1f8a423"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8482),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8483),
                            ModifiedBy = "System",
                            TableNumber = 2
                        },
                        new
                        {
                            Id = new Guid("ec0fad21-c060-4315-a39f-6947deccd8fa"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8560),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8561),
                            ModifiedBy = "System",
                            TableNumber = 3
                        },
                        new
                        {
                            Id = new Guid("735e6fee-be38-4b02-a1e9-659e727c072e"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8572),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8573),
                            ModifiedBy = "System",
                            TableNumber = 4
                        },
                        new
                        {
                            Id = new Guid("45694a13-30c1-4ff9-b7d2-8079657a6e29"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8578),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8579),
                            ModifiedBy = "System",
                            TableNumber = 5
                        },
                        new
                        {
                            Id = new Guid("87cbce7d-0daa-4ed5-9473-a1a473ca0cb5"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8592),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8593),
                            ModifiedBy = "System",
                            TableNumber = 6
                        },
                        new
                        {
                            Id = new Guid("2a150b61-8f9a-497f-a77a-2c701158b5a5"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8603),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8604),
                            ModifiedBy = "System",
                            TableNumber = 7
                        },
                        new
                        {
                            Id = new Guid("830f9c25-7cbd-44b7-84b1-bd55973deca9"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8608),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8609),
                            ModifiedBy = "System",
                            TableNumber = 8
                        },
                        new
                        {
                            Id = new Guid("3cd0f023-94e8-4114-b139-21d4955e1bab"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8619),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8620),
                            ModifiedBy = "System",
                            TableNumber = 9
                        },
                        new
                        {
                            Id = new Guid("a6ce6962-20ec-4def-be6d-d568a12a022c"),
                            CreatedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8630),
                            CreatedBy = "System",
                            IsDeleted = false,
                            IsInUse = false,
                            ModifiedAtUtc = new DateTime(2023, 1, 19, 5, 23, 55, 974, DateTimeKind.Utc).AddTicks(8631),
                            ModifiedBy = "System",
                            TableNumber = 10
                        });
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.MenuItem", b =>
                {
                    b.HasOne("Lewee.Domain.EnumEntity<Sample.Restaurant.Contracts.MenuItemType>", "ItemType")
                        .WithMany()
                        .HasForeignKey("ItemTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ItemType");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.Order", b =>
                {
                    b.HasOne("Lewee.Domain.EnumEntity<Sample.Restaurant.Contracts.OrderStatus>", "OrderStatus")
                        .WithMany()
                        .HasForeignKey("OrderStatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Sample.Restaurant.Domain.Table", "Table")
                        .WithMany("Orders")
                        .HasForeignKey("TableId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("OrderStatus");

                    b.Navigation("Table");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.OrderItem", b =>
                {
                    b.HasOne("Sample.Restaurant.Domain.MenuItem", "MenuItem")
                        .WithMany()
                        .HasForeignKey("MenuItemId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Sample.Restaurant.Domain.Order", "Order")
                        .WithMany("Items")
                        .HasForeignKey("OrderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("MenuItem");

                    b.Navigation("Order");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.Order", b =>
                {
                    b.Navigation("Items");
                });

            modelBuilder.Entity("Sample.Restaurant.Domain.Table", b =>
                {
                    b.Navigation("Orders");
                });
#pragma warning restore 612, 618
        }
    }
}
