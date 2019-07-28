﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Models;

namespace VegankoService.Data
{
    public class VegankoContext : DbContext
    {
        public VegankoContext()
        {}

        public DbSet<Product> Products { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            //optionsBuilder.UseInMemoryDatabase("Products");
            var connection = @"Server=(localdb)\mssqllocaldb;Database=VegankoService;Trusted_Connection=True;ConnectRetryCount=0";
            optionsBuilder.UseSqlServer(connection);
        }
    }
}
