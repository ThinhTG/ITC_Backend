using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Revenue
{
	public class RevenueService : IRevenueService
	{
		private readonly IRevenueRepository _revenueRepository;
		private readonly IWalletTransactionRepository _walletTransactionRepository;

		public RevenueService(IRevenueRepository revenueRepository, IWalletTransactionRepository walletTransactionRepository)
		{
			_revenueRepository = revenueRepository;
			_walletTransactionRepository = walletTransactionRepository;
		}

		public async Task<RevenueReportDto> GetRevenueReportAsync()
		{
			return new RevenueReportDto
			{
				TotalServiceFee = await _revenueRepository.GetTotalServiceFeeAsync(),
				//PackageRevenues = await _revenueRepository.GetPackageRevenueAsync(),
				//TotalCustomers = await _revenueRepository.GetTotalCustomersAsync(),
				TotalJobPosts = await _revenueRepository.GetTotalJobPostsAsync(),
				//	TopBPVs = await _revenueRepository.GetTopBookedBPVsAsync(3)
			};
		}

		public async Task<DashboardDto> GetDashboardAsync()
		{
			var totalRevenue = await _revenueRepository.GetTotalServiceFeeAsync();
			var now = DateTime.UtcNow;
			var monthlyRevenue = await _walletTransactionRepository.GetMonthlyRevenueAsync(now.Month, now.Year);
			var totalTransactions = await _walletTransactionRepository.GetTotalTransactionsAsync();
			return new DashboardDto
			{
				TotalRevenue = totalRevenue,
				MonthlyRevenue = monthlyRevenue,
				TotalTransactions = totalTransactions
			};
		}
	}

}
