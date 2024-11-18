﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Oracle.EntityFrameworkCore.Metadata;
using ShahinApis.Data;

#nullable disable

namespace ShahinApis.Migrations
{
    [DbContext(typeof(ShahinDbContext))]
    [Migration("20241118124641_Initial")]
    partial class Initial
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            OracleModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ShahinApis.Data.Model.ShahinReqLog", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("JsonReq")
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<DateTime>("LogDateTime")
                        .HasColumnType("TIMESTAMP(7)");

                    b.Property<string>("PublicAppId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PublicReqId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ServiceId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.ToTable("Shahin_Req", (string)null);
                });

            modelBuilder.Entity("ShahinApis.Data.Model.ShahinResLog", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("HTTPStatusCode")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("JsonRes")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("PublicReqId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.Property<string>("ReqLogId")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(450)");

                    b.Property<string>("ResCode")
                        .IsRequired()
                        .HasColumnType("NVARCHAR2(2000)");

                    b.HasKey("Id");

                    b.HasIndex("Id")
                        .IsUnique();

                    b.HasIndex("ReqLogId")
                        .IsUnique();

                    b.ToTable("Shahin_Res", (string)null);
                });

            modelBuilder.Entity("ShahinApis.Data.Model.ShahinResLog", b =>
                {
                    b.HasOne("ShahinApis.Data.Model.ShahinReqLog", "ReqLog")
                        .WithMany("ShahinResLogs")
                        .HasForeignKey("ReqLogId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("ReqLog");
                });

            modelBuilder.Entity("ShahinApis.Data.Model.ShahinReqLog", b =>
                {
                    b.Navigation("ShahinResLogs");
                });
#pragma warning restore 612, 618
        }
    }
}
