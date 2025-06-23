	using ITC.BusinessObject.Entities;
	using ITC.BusinessObject.Identity;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;

	namespace ITC.Repositories.Base
	{
		public class ITCDbContext : IdentityDbContext<
			 ApplicationUser,
			 ApplicationRole,
			 Guid,
			 ApplicationUserClaims,
			 ApplicationUserRole,
			 ApplicationUserLogins,
			 ApplicationRoleClaims,
			 ApplicationUserTokens>
		{
			public ITCDbContext(DbContextOptions<ITCDbContext> options) : base(options) {
			this.Database.Migrate();
		}

			// user
			public virtual DbSet<ApplicationUser> ApplicationUsers => Set<ApplicationUser>();
			public virtual DbSet<ApplicationRole> ApplicationRoles => Set<ApplicationRole>();
			public virtual DbSet<ApplicationUserClaims> ApplicationUserClaims => Set<ApplicationUserClaims>();
			public virtual DbSet<ApplicationUserRole> ApplicationUserRoles => Set<ApplicationUserRole>();
			public virtual DbSet<ApplicationUserLogins> ApplicationUserLogins => Set<ApplicationUserLogins>();
			public virtual DbSet<ApplicationRoleClaims> ApplicationRoleClaims => Set<ApplicationRoleClaims>();
			public virtual DbSet<ApplicationUserTokens> ApplicationUserTokens => Set<ApplicationUserTokens>();
			public virtual DbSet<Wallet> Wallets => Set<Wallet>();

		    public virtual DbSet<WalletTransaction> WalletTransaction => Set<WalletTransaction>();

		    public virtual DbSet<Job> Jobs => Set<Job>();
			public virtual DbSet<JobApplication> JobApplications => Set<JobApplication>();
		public virtual DbSet<TranslatorCertificate> TranslatorCertificates => Set<TranslatorCertificate>();

		public virtual DbSet<SubscriptionPlan> SubscriptionPlans => Set<SubscriptionPlan>();

		public virtual DbSet<UserSubscription> UserSubscriptions => Set<UserSubscription>();

		public virtual DbSet<Notification> Notifications => Set<Notification>();

		public virtual DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();

		public virtual DbSet<Review> Reviews => Set<Review>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
			{
				base.OnModelCreating(modelBuilder);

				foreach (var entityType in modelBuilder.Model.GetEntityTypes())
				{
					string tableName = entityType.GetTableName() ?? "";
					if (tableName.StartsWith("AspNet"))
					{
						entityType.SetTableName(tableName.Substring(6));
					}
				}

				modelBuilder.Entity<ApplicationUserRole>()
					.HasOne(ur => ur.User)
					.WithMany(u => u.UserRoles)
					.HasForeignKey(ur => ur.UserId)
					.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<ApplicationUserRole>(b =>
			{
				b.HasKey(ur => new { ur.UserId, ur.RoleId }); 

				b.HasOne(ur => ur.User)
					.WithMany(u => u.UserRoles)
					.HasForeignKey(ur => ur.UserId)
					.OnDelete(DeleteBehavior.Cascade);

				b.HasOne(ur => ur.Role)
					.WithMany()
					.HasForeignKey(ur => ur.RoleId)
					.OnDelete(DeleteBehavior.NoAction);
			});

			modelBuilder.Entity<Job>()
	.HasOne(j => j.Customer)
	.WithMany()
	.HasForeignKey(j => j.CustomerId)
	.OnDelete(DeleteBehavior.Restrict);

			
			modelBuilder.Entity<TranslatorCertificate>()
				.HasOne(tc => tc.User)
				.WithMany(u => u.TranslatorCertificates)
				.HasForeignKey(tc => tc.ApplicationUserId)
				.OnDelete(DeleteBehavior.Cascade);


			modelBuilder.Entity<SubscriptionPlan>().HasKey(sp => sp.Id);
			modelBuilder.Entity<UserSubscription>().HasKey(us => us.Id);

			modelBuilder.Entity<UserSubscription>()
				.HasOne(us => us.User)
				.WithMany()
				.HasForeignKey(us => us.UserId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<UserSubscription>()
				.HasOne(us => us.SubscriptionPlan)
				.WithMany(sp => sp.UserSubscriptions)
				.HasForeignKey(us => us.SubscriptionPlanId)
				.OnDelete(DeleteBehavior.Cascade);

			modelBuilder.Entity<Notification>(entity =>
			{
				entity.HasKey(x => x.Id);
				entity.HasOne<ApplicationUser>()
					  .WithMany()
					  .HasForeignKey(x => x.ReceiverUserId)
					  .OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<WithdrawalRequest>(entity =>
			{
				entity.HasKey(wr => wr.WithdrawalRequestId);

				entity.HasOne(wr => wr.Account)
					  .WithMany(u => u.WithdrawalRequests)
					  .HasForeignKey(wr => wr.AccountId)
					  .OnDelete(DeleteBehavior.Cascade);
			});






		}
	}
	}
