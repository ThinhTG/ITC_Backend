using ITC.API.DI;
using ITC.API.Middleware;
using ITC.API.SeedData;
using ITC.BusinessObject.Identity;
using ITC.Core.Base;
using ITC.Core.Hubs;
using ITC.Services.User;
using Microsoft.AspNetCore.Identity;
using Net.payOS;
using System.IdentityModel.Tokens.Jwt;
using Newtonsoft.Json;

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
			builder.Services.AddControllers()
				.AddNewtonsoftJson(options =>
				{
					options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
					options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
				});
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();


			// SignalR Config 
			builder.Services.AddSignalR();


			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowFrontend", policy =>
				{
					policy.WithOrigins(
						"http://localhost:3000",
						"https://inter-trans-connect.web.app"
					)
					.AllowAnyHeader()
					.AllowAnyMethod()
					.AllowCredentials();
				});
			});


			// Dependency Injection
			builder.Services.InstallerServicesInAssembly(builder.Configuration);
			builder.Services.AddSingleton<JwtSecurityTokenHandler>();
			builder.Services.AddMemoryCache();
			builder.Services.Configure<UploadSettings>(
				builder.Configuration.GetSection("UploadSettings"));
			builder.Services.AddScoped<IUserService, UserService>();

			var app = builder.Build();

			// ? Enable CORS - ph?i ??t TR??C Authentication & Authorization
			app.UseCors("AllowFrontend");

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
					await SubscriptionPlanSeeder.SeedAsync(services);
					await User.Initialize(services, userManager, roleManager);
					Console.WriteLine("? Seed data initialized successfully.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"? Error seeding DB: {ex.Message}");
				}
			}

			app.MapHub<NotificationHub>("/hubs/notification");


			app.Run();
		}
	}
}
