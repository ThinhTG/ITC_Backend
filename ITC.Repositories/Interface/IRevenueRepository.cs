using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface IRevenueRepository
	{
		Task<decimal> GetTotalServiceFeeAsync();
		//Task<List<PackageRevenueDto>> GetPackageRevenueAsync();
		Task<int> GetTotalCustomersAsync();
		Task<int> GetTotalJobPostsAsync();
		//	Task<List<TopBPVDto>> GetTopBookedBPVsAsync(int top);
	}

}
