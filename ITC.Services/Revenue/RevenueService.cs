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

		public RevenueService(IRevenueRepository revenueRepository)
		{
			_revenueRepository = revenueRepository;
		}

		public async Task<RevenueReportDto> GetRevenueReportAsync()
		{
			return new RevenueReportDto
			{
				TotalServiceFee = await _revenueRepository.GetTotalServiceFeeAsync(),
				//PackageRevenues = await _revenueRepository.GetPackageRevenueAsync(),
				TotalCustomers = await _revenueRepository.GetTotalCustomersAsync(),
				TotalJobPosts = await _revenueRepository.GetTotalJobPostsAsync(),
				//	TopBPVs = await _revenueRepository.GetTopBookedBPVsAsync(3)
			};
		}
	}

}
