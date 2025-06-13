using ITC.BusinessObject.DTOs.User;
using ITC.BusinessObject.Identity;
using Microsoft.AspNetCore.Identity;

namespace ITC.Services.User
{
    public interface IUserService
    {
        Task<UserCountByRoleDto> GetUserCountByRoleAsync();
    }
} 