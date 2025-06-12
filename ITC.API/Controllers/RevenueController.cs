using ITC.Services.Revenue;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class RevenueController : ControllerBase
	{
		private readonly IRevenueService _revenueService;

		public RevenueController(IRevenueService revenueService)
		{
			_revenueService = revenueService;
		}

		[HttpGet("report")]
		public async Task<IActionResult> GetRevenueReport()
		{
			var report = await _revenueService.GetRevenueReportAsync();
			return Ok(report);
		}
	}
}
