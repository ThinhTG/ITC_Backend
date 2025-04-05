
using AutoMapper;
using ITC.API.DI;
using ITC.API.SeedData;
using ITC.BusinessObject.Identity;
using ITC.Core.Mapping;
using Microsoft.AspNetCore.Identity;
using System.IdentityModel.Tokens.Jwt;

namespace ITC.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<JwtSecurityTokenHandler>();
            // ??ng ký d?ch v? thông qua DI Installer
            builder.Services.InstallerServicesInAssembly(builder.Configuration);
            builder.Services.AddMemoryCache();

			builder.Services.AddAutoMapper(typeof(MappingProfile));
			// In Program.cs
			var mapper = builder.Services.BuildServiceProvider().GetRequiredService<IMapper>();
			mapper.ConfigurationProvider.AssertConfigurationIsValid();

			var app = builder.Build();

            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();

                await User.Initialize(services, userManager, roleManager);
                Console.WriteLine("Seed data initialized successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while seeding the database: {ex.Message}");
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
