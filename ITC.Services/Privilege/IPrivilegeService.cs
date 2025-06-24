using ITC.Core.Enum;
using System.Threading.Tasks;
using System;

namespace ITC.Services.Privilege
{
    public interface IPrivilegeService
    {
        // Customer
        Task<bool> CanPostJobAsync(Guid userId);
        Task<int> GetRemainingJobPostsAsync(Guid userId);
        Task<decimal> GetServiceFeePercentageAsync(Guid userId);

        // Talent
        Task<bool> CanApplyJobAsync(Guid userId);
        Task<int> GetRemainingApplicationsAsync(Guid userId);
        Task<decimal> GetCommissionFeePercentageAsync(Guid userId);

        // Boosted
        Task<bool> IsUserBoostedAsync(Guid userId);
    }
} 