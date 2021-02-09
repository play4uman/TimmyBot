﻿// <auto-generated />
using System;
using Files.DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Files.Migrations
{
    [DbContext(typeof(FilesDbContext))]
    [Migration("20210209143635_FixFileNameType")]
    partial class FixFileNameType
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.2");

            modelBuilder.Entity("FileMetadataFileTag", b =>
                {
                    b.Property<Guid>("TaggedFilesId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("TagsTag")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("TaggedFilesId", "TagsTag");

                    b.HasIndex("TagsTag");

                    b.ToTable("FileMetadataFileTag");
                });

            modelBuilder.Entity("Files.DAL.Models.FileMetadata", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("FileName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("WordCount")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("FileMetadata");
                });

            modelBuilder.Entity("Files.DAL.Models.FileTag", b =>
                {
                    b.Property<string>("Tag")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Tag");

                    b.ToTable("FileTags");
                });

            modelBuilder.Entity("FileMetadataFileTag", b =>
                {
                    b.HasOne("Files.DAL.Models.FileMetadata", null)
                        .WithMany()
                        .HasForeignKey("TaggedFilesId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Files.DAL.Models.FileTag", null)
                        .WithMany()
                        .HasForeignKey("TagsTag")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
