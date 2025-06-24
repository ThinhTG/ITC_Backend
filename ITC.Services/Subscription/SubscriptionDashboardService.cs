using ITC.Repositories.Interface;

namespace ITC.Services.Subscription
{
	public class SubscriptionDashboardService : ISubscriptionDashboardService
    {
        private readonly IUserSubscriptionRepository _userSubRepo;
        private readonly ISubscriptionPlanRepository _planRepo;

        public SubscriptionDashboardService(IUserSubscriptionRepository userSubRepo, ISubscriptionPlanRepository planRepo)
        {
            _userSubRepo = userSubRepo;
            _planRepo = planRepo;
        }

        public async Task<SubscriptionDashboardDto> GetDashboardAsync(DateTime? from, DateTime? to)
        {
            var allSubs = await _userSubRepo.GetAllAsync();
            var query = allSubs.AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(s => s.SubscribedAt >= from);
            }
            if (to.HasValue)
            {
                query = query.Where(s => s.SubscribedAt <= to);
            }
            
            var filtered = query.ToList();

            var total = filtered.Count;
            var active = filtered.Count(s => s.IsActive && s.ExpiredAt > DateTime.UtcNow);
            var revenue = filtered.Sum(s => s.SubscriptionPlan?.Price ?? 0);

            var distribution = filtered
                .GroupBy(s => s.SubscriptionPlan?.Name ?? "Unknown")
                .Select(g => new PlanDistributionDto
                {
                    Plan = g.Key,
                    Count = g.Count(),
                    Percent = total > 0 ? Math.Round((double)g.Count() * 100 / total, 1) : 0
                })
                .ToList();

            var trend = filtered
                .GroupBy(s => s.SubscribedAt.Date)
                .Select(g => new SubscriptionTrendDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToList();

            var activeList = filtered
                .Where(s => s.IsActive && s.ExpiredAt > DateTime.UtcNow)
                .Select(s => new ActiveSubscriptionDto
                {
                    Customer = s.User?.FullName ?? s.UserId.ToString(),
                    Plan = s.SubscriptionPlan?.Name ?? "Unknown",
                    StartDate = s.SubscribedAt,
                    EndDate = s.ExpiredAt,
                    Amount = s.SubscriptionPlan?.Price ?? 0,
                    Status = "Active",
                    Payment = "Online" // Giả sử
                })
                .ToList();

            return new SubscriptionDashboardDto
            {
                TotalSubscriptions = total,
                ActiveSubscriptions = active,
                SubscriptionRevenue = revenue,
                DistributionByPlan = distribution,
                TrendOverTime = trend,
                ActiveSubscriptionList = activeList
            };
        }
    }
} 