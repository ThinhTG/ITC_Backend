using ITC.BusinessObject.DTOs.User;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.BusinessObject.Response;
using Microsoft.AspNetCore.Identity;

namespace ITC.Services.User
{
    public interface IUserService
    {
        Task<UserCountByRoleDto> GetUserCountByRoleAsync();
        Task<IEnumerable<UserResponse>> GetPendingApprovalUsersAsync();
        Task<bool> ApproveUserAsync(Guid userId);
        Task<bool> RejectUserAsync(Guid userId, RejectUserRequest request);
    }
} 