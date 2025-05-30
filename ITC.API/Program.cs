using ITC.API.DI;
using ITC.API.Middleware;
using ITC.API.SeedData;
using ITC.BusinessObject.Identity;
using ITC.Core.Base;
using Microsoft.AspNetCore.Identity;
using Net.payOS;
using System.IdentityModel.Tokens.Jwt;

namespace ITC.API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);
			IConfiguration configuration = new ConfigurationBuilder()
	.AddJsonFile("appsettings.json")
	.Build();
			PayOS payOS = new PayOS(configuration["PaymentEnvironment:PAYOS_CLIENT_ID"] ?? throw new Exception("Cannot find payment environment"),
								configuration["PaymentEnvironment:PAYOS_API_KEY"] ?? throw new Exception("Cannot find payment environment"),
								configuration["PaymentEnvironment:PAYOS_CHECKSUM_KEY"] ?? throw new Exception("Cannot find payment environment"));
			builder.Services.AddSingleton(payOS);


			// Add services to the container.
			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			// Dependency Injection
			builder.Services.InstallerServicesInAssembly(builder.Configuration);
			builder.Services.AddSingleton<JwtSecurityTokenHandler>();
			builder.Services.AddMemoryCache();
			builder.Services.Configure<UploadSettings>(
				builder.Configuration.GetSection("UploadSettings"));

			// ? Add CORS
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll", policy =>
				{
					policy.AllowAnyOrigin()
						  .AllowAnyHeader()
						  .AllowAnyMethod();
				});
			});

			var app = builder.Build();

			// ? Enable CORS - ph?i ??t TR??C Authentication & Authorization
			app.UseCors("AllowAll");

			// Swagger
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			//// Middlewares
			//app.UseHttpsRedirection();
			app.UseMiddleware<ExceptionHandlingMiddleware>();

			app.UseAuthentication();
			app.UseAuthorization();

			app.MapControllers();

			// ? Seed Data
			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
					var roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
					await User.Initialize(services, userManager, roleManager);
					Console.WriteLine("? Seed data initialized successfully.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"? Error seeding DB: {ex.Message}");
				}
			}

			app.Run();
		}
	}
}
