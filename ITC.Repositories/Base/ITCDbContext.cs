	using ITC.BusinessObject.Entities;
	using ITC.BusinessObject.Identity;
	using Microsoft.AspNetCore.Identity;
	using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
	using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL;
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
			//Database.Migrate();
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

		public virtual DbSet<Notifications> Notifications => Set<Notifications>();

		public virtual DbSet<WithdrawalRequest> WithdrawalRequests => Set<WithdrawalRequest>();

		public virtual DbSet<Review> Reviews => Set<Review>();

		public virtual DbSet<Complaint> Complaints => Set<Complaint>();
		public virtual DbSet<ComplaintMessage> ComplaintMessages => Set<ComplaintMessage>();

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

			modelBuilder.Entity<Notifications>(entity =>
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

			// Configure properties for PostgreSQL compatibility
			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(DateTimeOffset) || property.ClrType == typeof(DateTimeOffset?))
					{
						// Use timestamp with time zone for PostgreSQL
						property.SetColumnType("timestamp with time zone");
					}
					// Configure decimal properties with precision and scale
					else if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
					{
						property.SetPrecision(18);
						property.SetScale(2);
					}
					// Configure string properties to use text type in PostgreSQL
					else if (property.ClrType == typeof(string) && !property.IsPrimaryKey())
					{
						property.SetColumnType("text");
					}
				}
			}

			foreach (var entityType in modelBuilder.Model.GetEntityTypes())
			{
				foreach (var property in entityType.GetProperties())
				{
					if (property.ClrType == typeof(DateTimeOffset))
					{
						var converter = new ValueConverter<DateTimeOffset, DateTimeOffset>(
							v => v.ToOffset(TimeSpan.Zero), // Khi lưu -> UTC
							v => v                          // Khi đọc ra giữ nguyên UTC
						);
						property.SetColumnType("timestamp with time zone");
						property.SetValueConverter(converter);
					}
					else if (property.ClrType == typeof(DateTimeOffset?))
					{
						var converter = new ValueConverter<DateTimeOffset?, DateTimeOffset?>(
							v => v.HasValue ? v.Value.ToOffset(TimeSpan.Zero) : v,
							v => v
						);
						property.SetColumnType("timestamp with time zone");
						property.SetValueConverter(converter);
					}
					else if (property.ClrType == typeof(decimal) || property.ClrType == typeof(decimal?))
					{
						property.SetPrecision(18);
						property.SetScale(2);
					}
					else if (property.ClrType == typeof(string) && !property.IsPrimaryKey())
					{
						property.SetColumnType("text");
					}
				}
			}




		}
	}
	}
