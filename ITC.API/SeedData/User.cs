using ITC.BusinessObject.Identity;
using Microsoft.AspNetCore.Identity;

namespace ITC.API.SeedData
{
	public class User
	{
		public static async Task Initialize(IServiceProvider serviceProvider, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
		{
			// Cập nhật danh sách roleNames với 3 vai trò mới
			var roleNames = new[] { "Admin", "Customer", "Talent" };
			foreach (var roleName in roleNames)
			{
				var roleExist = await roleManager.RoleExistsAsync(roleName);
				if (!roleExist)
				{
					// Tạo mới role với Name và FullName (nếu cần)
					var newRole = new ApplicationRole
					{
						Name = roleName,
						FullName = roleName // Có thể tùy chỉnh FullName nếu muốn
					};
					await roleManager.CreateAsync(newRole);
				}
			}

			// Seed Admin user
			var user = await userManager.FindByEmailAsync("admin@admin.com");
			if (user == null)
			{
				var adminUser = new ApplicationUser
				{
					UserName = "admin@admin.com",
					Email = "admin@admin.com",
					FullName = "Admin User"
				};

				var result = await userManager.CreateAsync(adminUser, "Test@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(adminUser, "Admin");
				}
			}

			// Seed Customer user
			user = await userManager.FindByEmailAsync("customer@customer.com");
			if (user == null)
			{
				var customerUser = new ApplicationUser
				{
					UserName = "customer@customer.com",
					Email = "customer@customer.com",
					FullName = "Customer User"
				};

				var result = await userManager.CreateAsync(customerUser, "Test@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(customerUser, "Customer");
				}
			}

			// Seed Talent user
			user = await userManager.FindByEmailAsync("talent@talent.com");
			if (user == null)
			{
				var talentUser = new ApplicationUser
				{
					UserName = "talent@talent.com",
					Email = "talent@talent.com",
					FullName = "Talent User"
				};

				var result = await userManager.CreateAsync(talentUser, "Test@123");
				if (result.Succeeded)
				{
					await userManager.AddToRoleAsync(talentUser, "Talent");
				}
			}
		}
	}
}