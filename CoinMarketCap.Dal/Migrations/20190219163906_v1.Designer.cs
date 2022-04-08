﻿// <auto-generated />
using System;
using CoinMarketCap.Dal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CoinMarketCap.Dal.Migrations
{
    [DbContext(typeof(CoinMarketCapContext))]
    [Migration("20190219163906_v1")]
    partial class v1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("CoinMarketCap.Dal.App", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Key")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Login")
                        .HasColumnType("varchar(100)");

                    b.HasKey("Id");

                    b.ToTable("Apps");
                });

            modelBuilder.Entity("CoinMarketCap.Dal.LogEntry", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Date");

                    b.Property<string>("Login")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Type")
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UserIp")
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Value");

                    b.HasKey("Id");

                    b.ToTable("LogEntries");
                });

            modelBuilder.Entity("CoinMarketCap.Dal.Quote", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency")
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Pair")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("Price");

                    b.Property<string>("Symbol")
                        .HasColumnType("varchar(10)");

                    b.Property<long>("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("Quotes");
                });

            modelBuilder.Entity("CoinMarketCap.Dal.QuoteHistory", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Currency")
                        .HasColumnType("varchar(10)");

                    b.Property<string>("Pair")
                        .HasColumnType("varchar(255)");

                    b.Property<decimal>("Price");

                    b.Property<string>("Symbol")
                        .HasColumnType("varchar(10)");

                    b.Property<long>("Timestamp");

                    b.HasKey("Id");

                    b.ToTable("QuoteHistories");
                });
#pragma warning restore 612, 618
        }
    }
}
