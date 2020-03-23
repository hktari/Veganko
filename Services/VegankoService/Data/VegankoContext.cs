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
using VegankoService.Models.Stores;
using Veganko.Common.Models.Products;

namespace VegankoService.Data
{
    public class VegankoContext : IdentityDbContext<ApplicationUser>
    {
        public VegankoContext(DbContextOptions<VegankoContext> dbContextOptions)
            : base(dbContextOptions)
        {
        }
        
        public VegankoContext()
        {
        }

        public DbSet<ProductModRequest> ProductModRequests { get; set; }

        public DbSet<ProductModRequestEvaluation> ProductModRequestEvaluations { get; set; }

        public DbSet<UnapprovedProduct> UnapprovedProducts { get; set; }
        
        public DbSet<Product> Product { get; set; }

        public DbSet<Comment> Comment { get; set; } 

        public DbSet<Customer> Customer { get; set; }

        public DbSet<OTP> OTPs { get; set; }

        public DbSet<Store> Store { get; set; }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);

        //    optionsBuilder.UseMySql(Configuration["DBConnection"])
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // TODO: remove if this is no longer relevant.
            // This is a workaround to support an older version of mariaDB
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

            modelBuilder.Entity<Store>(entity => entity.Property(m => m.Id).HasMaxLength(127));
            modelBuilder.Entity<Store>(entity => entity.Property(m => m.ProductId).HasMaxLength(127));
            modelBuilder.Entity<Store>(entity => entity.Property(m => m.Name).HasMaxLength(127));
            
            // Add index to product id since usage will be mostly 'get stores by product id'
            modelBuilder.Entity<Store>().HasIndex(entity => entity.ProductId);
        }
    }
}
