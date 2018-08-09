using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NoteIt.Core.Entities;
using NoteIt.Infrastructure.Data.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteIt.Infrastructure.Data
{
    public class NoteDbContext : DbContext
    {
        //private string ConnectionString { get; set; }

        public DbSet<Note> Notes { get; set; }
        public DbSet<NoteContent> Contents { get; set; }

        public NoteDbContext(DbContextOptions<NoteDbContext> options)
            : base(options)
        {
            //ConnectionString = connectionString;
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer(ConnectionString);
        //}

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfiguration(new NoteConfiguration());
            builder.ApplyConfiguration(new NoteContentConfiguration());
        }
    }
}
