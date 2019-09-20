using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;
using VegankoService.Models.User;
using VegankoService.Models.Comments;
using Microsoft.AspNetCore.Identity;

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.UserName).HasMaxLength(127));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedUserName).HasMaxLength(127));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.Email).HasMaxLength(127));
            modelBuilder.Entity<ApplicationUser>(entity => entity.Property(m => m.NormalizedEmail).HasMaxLength(127));

            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.Name).HasMaxLength(127));
            modelBuilder.Entity<IdentityRole>(entity => entity.Property(m => m.NormalizedName).HasMaxLength(127));

            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserLogin<string>>(entity => entity.Property(m => m.ProviderKey).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserRole<string>>(entity => entity.Property(m => m.RoleId).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.UserId).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.LoginProvider).HasMaxLength(127));
            modelBuilder.Entity<IdentityUserToken<string>>(entity => entity.Property(m => m.Name).HasMaxLength(127));

            modelBuilder.Entity<Customer>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            //modelBuilder.Entity<Customer>(entity => entity.Property(m => m.IdentityId).HasMaxLength(127));
            //modelBuilder.Entity<Customer>(entity => entity.Property(m => m.Description).HasMaxLength(127));
            modelBuilder.Entity<Customer>(entity => entity.Property(m => m.AvatarId).HasMaxLength(127));
            //modelBuilder.Entity<Customer>(entity => entity.Property(m => m.ProfileBackgroundId).HasMaxLength(127));
            //modelBuilder.Entity<Customer>(entity => entity.Property(m => m.Label).HasMaxLength(127));

            modelBuilder.Entity<Product>(entity => entity.Property(m => m.Id).HasMaxLength(127));

            modelBuilder.Entity<Comment>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            //modelBuilder.Entity<Comment>(entity => entity.Property(m => m.ProductId).HasMaxLength(127));
            //modelBuilder.Entity<Comment>(entity => entity.Property(m => m.UserId).HasMaxLength(127));

            modelBuilder.Entity<OTP>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            //modelBuilder.Entity<OTP>(entity => entity.Property(m => m.IdentityId).HasMaxLength(127));
        }
    }
}
