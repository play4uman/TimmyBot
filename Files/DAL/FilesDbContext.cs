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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }
}
