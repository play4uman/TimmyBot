using Files.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL
{
    public class FilesDbContext : DbContext
    {
        public FilesDbContext(DbContextOptions<FilesDbContext> options)
           : base(options)
        { }

        public DbSet<FileMetadata> FileMetadata { get; set; }
        public DbSet<FileTag> FileTags { get; set; }
        public DbSet<Keyword> Keywords { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FileMetadataKeyword>()
                .HasKey(fmkw => new { fmkw.FileId, fmkw.KeywordId });
            modelBuilder.Entity<FileMetadataKeyword>()
                .HasOne(fmkw => fmkw.File)
                .WithMany(fm => fm.AssociatedKeywords)
                .HasForeignKey(fmkw => fmkw.FileId);
            modelBuilder.Entity<FileMetadataKeyword>()
                .HasOne(fmkw => fmkw.Keyword)
                .WithMany(kw => kw.OfFiles)
                .HasForeignKey(fmkw => fmkw.KeywordId);
        }
    }
}
