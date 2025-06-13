using ITC.Core.Contracts;
using ITC.Core.Enum;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Repository
{
	public class RevenueRepository : IRevenueRepository
	{
		private readonly ITCDbContext _context;

		public RevenueRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task<decimal> GetTotalServiceFeeAsync()
		{
			return await _context.Jobs
				.Where(t => t.Status == (int)JobStatus.Completed)
				.SumAsync(t => t.TotalFee);
		}


		// Doanh thu này của các gói dịch vụ
		//public async Task<List<PackageRevenueDto>> GetPackageRevenueAsync()
		//{
		//	return await _context.Subscriptions
		//		.GroupBy(s => new { s.PackageName, s.PackagePrice })
		//		.Select(g => new PackageRevenueDto
		//		{
		//			PackageName = g.Key.PackageName,
		//			PackagePrice = g.Key.PackagePrice,
		//			TotalUsers = g.Count(),
		//			Revenue = g.Count() * g.Key.PackagePrice
		//		}).ToListAsync();
		//}

		//public async Task<int> GetTotalCustomersAsync()
		//{
		//	return await _context.Users
	 //       .CountAsync(u => u.UserRoles.Any(r => r.Role.Name == "Customer"));
		//}

		public async Task<int> GetTotalJobPostsAsync()
		{
			return await _context.Jobs.CountAsync();
		}

		//public async Task<List<TopBPVDto>> GetTopBookedBPVsAsync(int top)
		//{
		//	return await _context.Transactions
		//		.Where(t => t.Status == "Completed")
		//		.GroupBy(t => new { t.BPV.Id, t.BPV.FullName })
		//		.OrderByDescending(g => g.Count())
		//		.Take(top)
		//		.Select(g => new TopBPVDto
		//		{
		//			BPVId = g.Key.Id,
		//			BPVName = g.Key.FullName,
		//			BookedCount = g.Count()
		//		}).ToListAsync();
		//}
	}

}
