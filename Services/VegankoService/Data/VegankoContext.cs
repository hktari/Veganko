using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.User;
using VegankoService.Models.Comments;

namespace VegankoService.Data
{
    public class VegankoContext : IdentityDbContext<ApplicationUser>
    {
        public VegankoContext()
        {}

        public DbSet<Product> Product { get; set; }

        public DbSet<Comment> Comment { get; set; } 

        public DbSet<Customer> Customer { get; set; }

        public DbSet<OTP> OTPs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connection = @"server=localhost;user=root;password=borut123;database=Veganko;";
            optionsBuilder.UseMySql(connection);
        }
    }
}
