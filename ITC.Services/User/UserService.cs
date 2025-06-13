using ITC.BusinessObject.DTOs.User;
using ITC.BusinessObject.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITC.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<UserCountByRoleDto> GetUserCountByRoleAsync()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            var talentRole = await _roleManager.FindByNameAsync("Talent");

            var customerCount = await _userManager.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == customerRole.Id))
                .CountAsync();

            var talentCount = await _userManager.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == talentRole.Id))
                .CountAsync();

            return new UserCountByRoleDto
            {
                CustomerCount = customerCount,
                TalentCount = talentCount
            };
        }
    }
} 