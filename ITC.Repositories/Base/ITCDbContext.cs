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

		public virtual DbSet<Job> Jobs => Set<Job>();
		public virtual DbSet<JobApplication> JobApplications => Set<JobApplication>();


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

			modelBuilder.Entity<ApplicationUserRole>()
				.HasOne<ApplicationRole>()
				.WithMany()
				.HasForeignKey(ur => ur.RoleId)
				.OnDelete(DeleteBehavior.NoAction);

			// 🔽 Job
			modelBuilder.Entity<Job>(entity =>
			{
				entity.HasKey(j => j.Id);
				entity.Property(j => j.Title).IsRequired().HasMaxLength(255);
				entity.Property(j => j.Description).HasMaxLength(1000);
				entity.Property(j => j.Location).HasMaxLength(255);
				entity.Property(j => j.Status).HasDefaultValue(0);
				entity.Property(j => j.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

				entity.HasOne(j => j.Customer)
					.WithMany()
					.HasForeignKey(j => j.CustomerId)
					.OnDelete(DeleteBehavior.Restrict);
			});

			// 🔽 JobApplication
			modelBuilder.Entity<JobApplication>(entity =>
			{
				entity.HasKey(a => a.Id);
				entity.Property(a => a.Message).HasMaxLength(1000);
				entity.Property(a => a.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

				entity.HasOne(a => a.Job)
					.WithMany(j => j.Applications)
					.HasForeignKey(a => a.JobId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(a => a.Interpreter)
					.WithMany()
					.HasForeignKey(a => a.InterpreterId)
					.OnDelete(DeleteBehavior.Restrict);
			});


		}
    }
}
