namespace ITC.Services.Revenue
{
	public interface IRevenueDashboardService
    {
        Task<RevenueDashboardDto> GetDashboardAsync(DateTime? from, DateTime? to);
    }
} 