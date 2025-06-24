using ITC.Core.Enum;
using ITC.Repositories.Interface;
using System.Threading.Tasks;
using System;
using ITC.Services.Privilege;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace ITC.Services.Privilege
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepo;
        private readonly IJobRepository _jobRepo;
        private readonly IJobApplicationRepository _jobApplicationRepo;

        public PrivilegeService(IUserSubscriptionRepository userSubscriptionRepo, IJobRepository jobRepo, IJobApplicationRepository jobApplicationRepo)
        {
            _userSubscriptionRepo = userSubscriptionRepo;
            _jobRepo = jobRepo;
            _jobApplicationRepo = jobApplicationRepo;
        }

        public async Task<PrivilegeLevel> GetUserPrivilegeLevelAsync(Guid userId)
        {
            var activeSub = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);

            if (activeSub == null || activeSub.SubscriptionPlan == null)
            {
                return PrivilegeLevel.NoSubscription;
            }

            // Mapping from plan name to privilege level
            switch (activeSub.SubscriptionPlan.Name.ToLower())
            {
                case "partnership":
                    return PrivilegeLevel.PartnerShip;

                case "advance":
                    return PrivilegeLevel.Advance;

                case "premium":
                    return PrivilegeLevel.Premium;

                default:
                    return PrivilegeLevel.NoSubscription;
            }
        }

        // Customer
        public async Task<bool> CanPostJobAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return false; // Or handle based on a free plan logic
            var limit = subscription.SubscriptionPlan.JobPostLimit;
            if (limit == null) // Null means unlimited
                return true;
            var jobs = _jobRepo.GetJobsByCustomerIdQueryable(userId)
                .Where(j => j.CreatedAt >= subscription.SubscribedAt && j.CreatedAt <= subscription.ExpiredAt);
            int posted = await jobs.CountAsync();
            return posted < limit;
        }

        public async Task<int> GetRemainingJobPostsAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return 0;
            var limit = subscription.SubscriptionPlan.JobPostLimit;
            if (limit == null)
                return int.MaxValue; // Represents unlimited
            var jobs = _jobRepo.GetJobsByCustomerIdQueryable(userId)
                .Where(j => j.CreatedAt >= subscription.SubscribedAt && j.CreatedAt <= subscription.ExpiredAt);
            int posted = await jobs.CountAsync();
            return Math.Max(0, limit.Value - posted);
        }

        public async Task<decimal> GetServiceFeePercentageAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            // Return the fee from the plan, or a default value if no plan is active
            return subscription?.SubscriptionPlan?.ServiceFeePercentage ?? 0.2m; // Default 20%
        }

        // Talent
        public async Task<bool> CanApplyJobAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return false; // Or handle based on a free plan logic
            var limit = subscription.SubscriptionPlan.ApplicationLimit;
            if (limit == null) // Null means unlimited
                return true;

            var applications = _jobApplicationRepo.GetJobApplicationsByInterpreterIdQueryable(userId)
                .Where(a => a.CreatedAt >= subscription.SubscribedAt && a.CreatedAt <= subscription.ExpiredAt);

            int appliedCount = await applications.CountAsync();
            return appliedCount < limit;
        }

        public async Task<int> GetRemainingApplicationsAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return 0;
            var limit = subscription.SubscriptionPlan.ApplicationLimit;
            if (limit == null)
                return int.MaxValue; // Represents unlimited

            var applications = _jobApplicationRepo.GetJobApplicationsByInterpreterIdQueryable(userId)
                 .Where(a => a.CreatedAt >= subscription.SubscribedAt && a.CreatedAt <= subscription.ExpiredAt);

            int appliedCount = await applications.CountAsync();
            return Math.Max(0, limit.Value - appliedCount);
        }

        public async Task<decimal> GetCommissionFeePercentageAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            // Return the fee from the plan, or a default value if no plan is active
            return subscription?.SubscriptionPlan?.CommissionFeePercentage ?? 0.1m; // Default 10%
        }

        // Boosted
        public async Task<bool> IsUserBoostedAsync(Guid userId)
        {
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            return subscription?.SubscriptionPlan?.IsBoosted ?? false;
        }
    }
} 