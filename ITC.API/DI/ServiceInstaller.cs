using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
using ITC.Services.Auth;
using ITC.Services.Certificate;
using ITC.Services.Email;
using ITC.Services.JobApplyService;
using ITC.Services.JobService;
using ITC.Services.JobWork;
using ITC.Services.Notification;
using ITC.Services.PaymentService;
using ITC.Services.Revenue;
using ITC.Services.SubscriptionPlan;
using ITC.Services.TokenService;
using ITC.Services.User;
using ITC.Services.WalletService;
using ITC.Services.WithdrawalService;

namespace ITC.API.DI
{
	public class ServiceInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
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
			services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
			services.AddScoped<IJobApplicationService, JobApplicationService>();
			services.AddScoped<IPaymentService, PaymentService>();
			services.AddScoped<IEmailService, EmailService>();
			services.AddScoped<IJobWorkService, JobWorkService>();
			services.AddScoped<IRevenueRepository, RevenueRepository>();
			services.AddScoped<IRevenueService, RevenueService>();
			services.AddScoped<ITranslatorCertificateRepository, TranslatorCertificateRepository>();
			services.AddScoped<ITranslatorCertificateService, TranslatorCertificateService>();
			services.AddScoped<IUserSubscriptionRepository, UserSubscriptionRepository>();
			services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();
			services.AddScoped<IUserSubscriptionService, UserSubscriptionService>();
			services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();
			services.AddScoped<INotificationRepository, NotificationRepository>();
			services.AddScoped<INotificationService, NotificationService>();
			services.AddScoped<IWithdrawalRequestRepository, WithdrawalRequestRepository>();
			services.AddScoped<IWithdrawalRequestService, WithdrawalRequestService>();
			services.AddScoped<IUserService, UserService>();
			services.AddScoped<IReviewRepository, ReviewRepository>();
			services.AddScoped<IReviewService, ReviewService>();
		}
	}
}
