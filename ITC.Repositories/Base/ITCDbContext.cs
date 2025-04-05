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
        //public virtual DbSet<KeyPair> KeyPairs => Set<KeyPair>();
        //public virtual DbSet<KeyEndpoint> KeyEndpoints => Set<KeyEndpoint>();
        //public virtual DbSet<WhiteListIp> WhiteListIps => Set<WhiteListIp>();







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


		}
    }
}
