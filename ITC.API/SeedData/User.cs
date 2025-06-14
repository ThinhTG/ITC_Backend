using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITC.API.SeedData
{
	public class User
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			var roleNames = new[] { "Admin", "Customer", "Talent", "Staff" };

			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					var newRole = new ApplicationRole
					{
						Name = roleName,
						FullName = roleName
					};
					await roleManager.CreateAsync(newRole);
				}
			}

			using (var scope = serviceProvider.CreateScope())
			{
				var context = scope.ServiceProvider.GetRequiredService<ITCDbContext>();
				var walletRepo = scope.ServiceProvider.GetRequiredService<IWalletRepository>();

				async Task SeedUser(string email, string role, string fullName, string phone, string address)
				{
					var user = await userManager.FindByEmailAsync(email);
					if (user == null)
					{
						var newUser = new ApplicationUser
						{
							UserName = email,
							Email = email,
							FullName = fullName,
							PhoneNumber = phone,
							PhoneNumberConfirmed = true,
							Address = address
						};

						var result = await userManager.CreateAsync(newUser, "Test@123");
						if (result.Succeeded)
						{
							await userManager.AddToRoleAsync(newUser, role);

							if (role == "Customer" || role == "Staff")
							{
								var wallet = new Wallet
								{
									WalletId = Guid.NewGuid(),
									AccountId = newUser.Id,
									Balance = 0
								};
								await walletRepo.CreateWallet(wallet);
							}
						}
					}
				}

				await SeedUser("admin@admin.com", "Admin", "Admin User", "1234567890", "82 admin address");
				await SeedUser("customer@customer.com", "Customer", "Customer User", "0987654321", "82 customer address");
				await SeedUser("talent@talent.com", "Talent", "Talent User", "1122334455", "82 talent address");
				await SeedUser("staff@staff.com", "Staff", "Staff User", "5566778899", "82 staff address");

				// Seed subscription plans if not exist
				if (!await context.SubscriptionPlans.AnyAsync())
				{
					var plans = new List<SubscriptionPlan>
					{
						new SubscriptionPlan
						{
							Id = Guid.NewGuid(),
							Name = "PartnerShip",
							Price = 99000,
							Description = "Increase visibility in search results and appear highlighted.",
							DurationInDays = 30,
							CreatedAt = DateTime.UtcNow
						},
						new SubscriptionPlan
						{
							Id = Guid.NewGuid(),
							Name = "Advance",
							Price = 199000,
							Description = "Higher priority in recommendations and more prominent display than PartnerShip.",
							DurationInDays = 30,
							CreatedAt = DateTime.UtcNow
						},
						new SubscriptionPlan
						{
							Id = Guid.NewGuid(),
							Name = "Premium",
							Price = 299000,
							Description = "Unlock all premium features: top visibility, AI-based suggestions, and advanced filters.",
							DurationInDays = 30,
							CreatedAt = DateTime.UtcNow
						}
					};

					await context.SubscriptionPlans.AddRangeAsync(plans);
					await context.SaveChangesAsync();
				}
			}
		}
	}
}
