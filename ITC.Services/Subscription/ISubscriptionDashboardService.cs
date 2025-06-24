namespace ITC.Services.Subscription
{
	public interface ISubscriptionDashboardService
    {
        Task<SubscriptionDashboardDto> GetDashboardAsync(DateTime? from, DateTime? to);
    }
} 