using ITC.BusinessObject.Identity;
using ITC.Core.Contracts;
using ITC.Services.SubscriptionPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class SubscriptionController : ControllerBase
	{
		private readonly IUserSubscriptionService _subscriptionService;
		private readonly UserManager<ApplicationUser> _userManager;

		public SubscriptionController(IUserSubscriptionService subscriptionService, UserManager<ApplicationUser> userManager)
		{
			_subscriptionService = subscriptionService;
			_userManager = userManager;
		}


		/// <summary>
		/// Đăng ký gói subscription cho người dùng hiện tại.
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("subscribe")]
		[Authorize]
		public async Task<IActionResult> Subscribe([FromBody] SubscriptionRequestDto request)
		{
			var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var result = await _subscriptionService.SubscribeAsync(userId, request.SubscriptionPlanId);
			return Ok(result);
		}


		/// <summary>
		/// Lấy gói đăng ký hiện tại của người dùng.
		/// </summary>
		/// <returns></returns>
		[HttpGet("current")]
		[Authorize]
		public async Task<IActionResult> GetCurrent()
		{
			var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
			var result = await _subscriptionService.GetCurrentSubscriptionAsync(userId);
			if (result == null) return NotFound("Chưa có gói đăng ký");
			return Ok(result);
		}
	}

}
