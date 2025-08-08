using ITC.Core.Enum;
using ITC.Repositories.Interface;
using System.Threading.Tasks;
using System;
using ITC.Services.Privilege;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using ITC.BusinessObject.Identity;

namespace ITC.Services.Privilege
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepo;
        private readonly IJobRepository _jobRepo;
        private readonly IJobApplicationRepository _jobApplicationRepo;
        private readonly UserManager<ApplicationUser> _userManager;

        public PrivilegeService(
            IUserSubscriptionRepository userSubscriptionRepo, 
            IJobRepository jobRepo, 
            IJobApplicationRepository jobApplicationRepo,
            UserManager<ApplicationUser> userManager)
        {
            _userSubscriptionRepo = userSubscriptionRepo;
            _jobRepo = jobRepo;
            _jobApplicationRepo = jobApplicationRepo;
            _userManager = userManager;
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
            // Check if user has used less than 5 free job posts
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.FreeJobPostsUsed < 5)
            {
                return true; // Allow free posting
            }

            // If user has used all free posts, check subscription
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return false; // No subscription and no free posts left
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
            // Check if user has free posts remaining
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.FreeJobPostsUsed < 5)
            {
                return 5 - user.FreeJobPostsUsed; // Return remaining free posts
            }

            // If user has used all free posts, check subscription
            var subscription = await _userSubscriptionRepo.GetActiveSubscriptionAsync(userId);
            if (subscription == null || subscription.SubscriptionPlan == null)
                return 0; // No subscription and no free posts left
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

        // Free job posts management
        public async Task IncrementFreeJobPostsUsedAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null && user.FreeJobPostsUsed < 5)
            {
                user.FreeJobPostsUsed++;
                await _userManager.UpdateAsync(user);
            }
        }

        public async Task<int> GetFreeJobPostsUsedAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            return user?.FreeJobPostsUsed ?? 0;
        }

        public async Task<int> GetFreeJobPostsRemainingAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return 0;
            return Math.Max(0, 5 - user.FreeJobPostsUsed);
        }
    }
} 