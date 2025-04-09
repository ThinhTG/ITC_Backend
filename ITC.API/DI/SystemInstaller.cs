using ITC.BusinessObject.Identity;
using ITC.Core.Base;
using ITC.Mapping.Mapper;
using ITC.Repositories.Base;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace ITC.API.DI
{
	public class SystemInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            // Add DB Context
            services.AddDbContext<ITCDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });


            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder =>
                    {
                        builder.WithOrigins("*")
                               .AllowAnyHeader()
                               .AllowAnyMethod();
                    });
            });



            // Identity Configuration
            services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                options.SignIn.RequireConfirmedEmail = true;
            })
            .AddEntityFrameworkStores<ITCDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();



            // ✅ Đọc cấu hình JwtSettings đúng cách
            var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
            if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
            {
                throw new ArgumentNullException("JwtSettings", "JWT settings are missing in configuration.");
            }

            // ✅ Đăng ký JwtSettings vào DI container
            services.AddSingleton(jwtSettings);

            // ✅ Sử dụng JwtSettings đã đăng ký từ DI container
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey!)),
                    ClockSkew = TimeSpan.Zero
                };
                options.SaveToken = true;
                options.RequireHttpsMetadata = true;
            });

			// Register AutoMapper
			services.AddAutoMapper(typeof(MappingProfile).Assembly);

			// Authorization Policies
			services.AddAuthorization(options =>
			{
				options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
				options.AddPolicy("CustomerPolicy", policy => policy.RequireRole("Customer"));
				options.AddPolicy("TalentPolicy", policy => policy.RequireRole("Talent"));
			});

			// Swagger Configuration
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "InterTransConnect API",
					Version = "v1",
					Description = "Services to InterTransConnect Website"
				});

				// Thêm hỗ trợ XML Comments
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);

				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "Please enter a valid token in the following format: {your token here} do not add the word 'Bearer' before it."
				});

				c.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							},
							Scheme = "oauth2",
							Name = "Bearer",
							In = ParameterLocation.Header,
						},
						new List<string>()
					}
				});
			});

			services.AddControllers();


		}
    }

}