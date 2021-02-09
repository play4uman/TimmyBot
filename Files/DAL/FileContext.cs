using Files.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Files.DAL
{
    public class FileContext : DbContext
    {
        public FileContext(DbContextOptions<FileContext> options)
           : base(options)
        { }

        public DbSet<FileMetadata> FileMetadata { get; set; }
        public DbSet<FileTag> FileTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
        }
    }
}
