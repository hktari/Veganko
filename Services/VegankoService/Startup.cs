using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using VegankoService.Data;
using VegankoService.Data.Comments;
using VegankoService.Models.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection.Extensions;
using VegankoService.Auth;
using VegankoService.Models;
using VegankoService.Helpers;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using VegankoService.Services;
using VegankoService.Data.Users;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.FileProviders;
using System.IO;
using VegankoService.Data.Stores;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net.Mime;

namespace VegankoService
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddSingleton<IProductRepository, MockProductRepository>();
            services.AddScoped((sp) =>
            {
                DbContextOptionsBuilder dbOptsBuilder = new DbContextOptionsBuilder().UseMySql(Configuration["DBConnection"]);
                var dbContext = new VegankoContext(dbOptsBuilder.Options);
                return dbContext;
            });

            services.AddScoped<IStoresRepository, StoresRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICommentRepository, CommentRepository>();
            services.AddScoped<IUsersRepository, UsersRepository>();

            services.AddScoped<IJwtFactory, JwtFactory>();

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IEmailService, EmailService>();
            //services.Configure<AuthMessageSenderOptions>(Configuration); ???

            // jwt wire up
            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            SymmetricSecurityKey _signingKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(Configuration["SecretKey"]));
        
            // Configure JwtIssuerOptions
            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,
                RoleClaimType  = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role",
                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            // api user claim policy
            services.AddAuthorization(options =>
            {
                options.AddPolicy("ApiUser", policy =>  policy.RequireClaim(Constants.Strings.JwtClaimIdentifiers.Rol, Constants.Strings.JwtClaims.ApiAccess));
            });

            // add identity
            var builder = services.AddIdentityCore<ApplicationUser>(o =>
            {
                o.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                // configure identity options
                o.Password.RequireDigit = true;
                o.Password.RequireLowercase = true;
                o.Password.RequireUppercase = true;
                o.Password.RequireNonAlphanumeric = true;
                o.Password.RequiredLength = 6;
                o.User.RequireUniqueEmail = true;
            })
            .AddRoles<IdentityRole>();
            //builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddEntityFrameworkStores<VegankoContext>().AddDefaultTokenProviders();

            services.Configure<FormOptions>(options =>
            {
                // Set the limit to 17 MB
                options.MultipartBodyLengthLimit = 16383000;
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = context =>
                {
                    var result = new BadRequestObjectResult(
                        new ValidationProblemDetails(context.ModelState)
                        {
                            Status = (int)HttpStatusCode.BadRequest,
                            Instance = context.HttpContext?.Request?.Path,
                        });
                    
                    result.ContentTypes.Add(MediaTypeNames.Application.Json);
                    
                    return result;
                };
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // TODO: turn on after certificate and domain 
                //app.UseHsts();

            }

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), @"..", @"images");
            Directory.CreateDirectory(imagesFolder);
            app.UseStaticFiles(new StaticFileOptions()
            {

                FileProvider = new PhysicalFileProvider(imagesFolder),
                RequestPath = new PathString("/images")
            });
            
            // TODO: turn on after certificate and domain 
            //app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute("default", "api/{controller=Home}/{action=Index}/{id?}");
            });

            UpdateDatabase(app);

            CreateRoles(serviceProvider).Wait();
        }

        private void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices
                .GetRequiredService<IServiceScopeFactory>()
                .CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<VegankoContext>())
                {
                    context.Database.Migrate();
                }
            }
        }

        private async Task CreateRoles(IServiceProvider serviceProvider)
        {
            //initializing custom roles 
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<VegankoContext>();

            string[] roleNames = { Constants.Strings.Roles.Admin, Constants.Strings.Roles.Manager, Constants.Strings.Roles.Moderator, Constants.Strings.Roles.Member };
            IdentityResult roleResult;

            foreach (var roleName in roleNames)
            {
                var roleExist = await RoleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    //create the roles and seed them to the database: Question 1
                    roleResult = await RoleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            var _user = await UserManager.FindByEmailAsync(Configuration["AdminUserEmail"]);
            if (_user == null)
            {
                //Here you could create a super user who will maintain the web app
                var poweruser = new ApplicationUser
                {
                    UserName = Configuration["AdminUsername"],
                    Email = Configuration["AdminUserEmail"],
                    EmailConfirmed = true
                };

                //Ensure you have these values in your appsettings.json file
                string userPWD = Configuration["AdminPassword"];

                var createPowerUser = await UserManager.CreateAsync(poweruser, userPWD);

                if (createPowerUser.Succeeded)
                {
                    //here we tie the new user to the role
                    await UserManager.AddToRoleAsync(poweruser, Constants.Strings.Roles.Admin);

                    context.Customer.Add(
                        new Customer
                        {
                            IdentityId = poweruser.Id,
                            AvatarId = "0",
                            ProfileBackgroundId = "0",
                        });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
