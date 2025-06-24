using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class RevenueReportDto
	{
		public decimal TotalServiceFee { get; set; }
		public List<PackageRevenueDto> PackageRevenues { get; set; }
		public int TotalCustomers { get; set; }
		public int TotalJobPosts { get; set; }
		public List<TopBPVDto> TopBPVs { get; set; }
	}

	public class PackageRevenueDto
	{
		public string PackageName { get; set; }
		public int TotalUsers { get; set; }
		public decimal PackagePrice { get; set; }
		public decimal Revenue { get; set; }
	}

	public class TopBPVDto
	{
		public int BPVId { get; set; }
		public string BPVName { get; set; }
		public int BookedCount { get; set; }
	}

	public class DashboardDto
	{
		public decimal TotalRevenue { get; set; }
		public decimal MonthlyRevenue { get; set; }
		public int TotalTransactions { get; set; }
	}
}
