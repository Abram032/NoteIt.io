using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace NoteIt.Data
{
    public class ApplicationIdentityDbContext : IdentityDbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationIdentityDbContext(DbContextOptions<ApplicationIdentityDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if(_configuration["ASPNETCORE_ENVIRONMENT"].Equals("Development"))
                optionsBuilder.UseInMemoryDatabase("Identity");
            else
                optionsBuilder.UseSqlServer(_configuration.GetConnectionString("DefaultConnection"));
        }
    }
}
