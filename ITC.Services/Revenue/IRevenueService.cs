using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Revenue
{
	public interface IRevenueService
	{
		Task<RevenueReportDto> GetRevenueReportAsync();
	}

}
