using ITC.Services.SubscriptionPlan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubPlanController : ControllerBase
	{
		private readonly ISubscriptionPlanService _subscriptionPlanService;

		public SubPlanController(ISubscriptionPlanService subscriptionPlanService)
		{
			_subscriptionPlanService = subscriptionPlanService;
		}

		/// <summary>
		/// Lấy các gói subscription hiện có.
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAllPlans()
		{
			var plans = await _subscriptionPlanService.GetAllPlansAsync();
			return Ok(plans);
		}

		/// <summary>
		/// Lấy gói đăng kí theo Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetPlanById(Guid id)
		{
			var plan = await _subscriptionPlanService.GetPlanByIdAsync(id);
			if (plan == null)
				return NotFound();

			return Ok(plan);
		}
	}
}
