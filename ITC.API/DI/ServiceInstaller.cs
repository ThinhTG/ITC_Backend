using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
using ITC.Services.Auth;
using ITC.Services.JobService;
using ITC.Services.OrderService;
using ITC.Services.TokenService;
using ITC.Services.WalletService;


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
			services.AddScoped<IWalletRepository, WalletRepository>();
			services.AddScoped<IWalletTransactionRepository, WalletTransactionRepo>();
			services.AddScoped<IWalletService, WalletService>();
			services.AddScoped<IWalletTransactionService, WalletTransactionService>();
			services.AddScoped<IOrderRepository, OrderRepository>();
			services.AddScoped<IOrderService, OrderService>();
		

		}
	}
}
