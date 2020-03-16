using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VegankoService.Data;
using Microsoft.EntityFrameworkCore;

namespace VegankoService.Services
{
	/// <summary>
    /// Required by EF commands like adding migrations and updating database.
    /// </summary>
    public class VegankoContextDesignTimeFactory : IDesignTimeDbContextFactory<VegankoContext>
    {
        public VegankoContext CreateDbContext(string[] args)
        {
            var dbOptsBuilder = new DbContextOptionsBuilder<VegankoContext>().UseMySql("server=localhost;user=root");
            var dbContext = new VegankoContext(dbOptsBuilder.Options);
            return dbContext;
        }
    }
}
