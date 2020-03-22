using System;
using System.Net.Http;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using VegankoService.Data;
using VegankoService.Models.User;
using VegankoService.Services;
using VegankoService.Tests.Services;

namespace VegankoService.Tests
{
    public class CustomWebApplicationFactory<TStartup> 
        : WebApplicationFactory<TStartup> where TStartup: class
    {
        public string FakeUserRole { get; set; } = VegankoService.Helpers.Constants.Strings.Roles.Admin;

        public Func<VegankoContext> CreateDbContext { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Create a new service provider.
                var serviceProvider = new ServiceCollection()
                    .AddEntityFrameworkInMemoryDatabase()
                    .BuildServiceProvider();
              
                // Add a database context (ApplicationDbContext) using an in-memory 
                // database for testing.
                services.AddDbContext<VegankoContext>(options => 
                {
                    options.UseInMemoryDatabase("InMemoryDbForTesting");
                    options.UseInternalServiceProvider(serviceProvider);
                });
                
                // Build the service provider.
                var sp = services.BuildServiceProvider();

                services.AddMvc(opts =>
                {
                    opts.Filters.Add(new AllowAnonymousFilter());
                    opts.Filters.Add(new FakeUserFilter(FakeUserRole));
                });

                CreateDbContext = () =>
                {
                    var scope = services.BuildServiceProvider().CreateScope();
                        return scope.ServiceProvider.GetRequiredService<VegankoContext>();
                };

                // Create a scope to obtain a reference to the database
                // context (ApplicationDbContext).
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<VegankoContext>();
                    var logger = scopedServices
                        .GetRequiredService<ILogger<CustomWebApplicationFactory<TStartup>>>();

                    // Ensure the database is created.
                    db.Database.EnsureCreated();

                    try
                    {
                        // Seed the database with test data.
                        Util.InitializeDbForTests(db);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "An error occurred seeding the database. Error: {Message}", ex.Message);
                    }
                }
            });

            builder.ConfigureTestServices(services =>
            {
                OnConfigureTestServices(services);
            });
        }

        protected virtual void OnConfigureTestServices(IServiceCollection services)
        {
            services.AddScoped<IEmailService, MockEmailService>();
        }
    }
}
