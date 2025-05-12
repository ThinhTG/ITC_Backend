using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
using ITC.Services.Auth;
using ITC.Services.JobService;
using ITC.Services.TokenService;


namespace ITC.API.DI
{
	public class ServiceInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
			services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IJobService, JobService>();
			services.AddScoped<IJobRepository, JobRepository>();

		}
	}
}
