using ITC.Services.DTOs.Withdrawal;
using ITC.Services.WithdrawalService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ITC.API.Controllers
{
    [Route("api/withdrawal-requests")]
    [ApiController]
    public class WithdrawalRequestController : ControllerBase
    {
        private readonly IWithdrawalRequestService _withdrawalRequestService;
        private readonly ILogger<WithdrawalRequestController> _logger;

        public WithdrawalRequestController(
            IWithdrawalRequestService withdrawalRequestService,
            ILogger<WithdrawalRequestController> logger)
        {
            _withdrawalRequestService = withdrawalRequestService;
            _logger = logger;
        }



		[HttpPost]
		[Authorize]
		public async Task<IActionResult> CreateWithdrawalRequest([FromBody] CreateWithdrawalRequestDto dto)
		{
			try
			{
				var accountIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				if (string.IsNullOrEmpty(accountIdClaim) || !Guid.TryParse(accountIdClaim, out var accountId))
				{
					return Unauthorized(new { Message = "Invalid or missing account ID" });
				}

				var result = await _withdrawalRequestService.CreateAsync(accountId, dto);
				return Ok(result);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error creating withdrawal request");
				return BadRequest(new { Message = ex.Message });
			}
		}


		/// <summary>
		/// L?y t?t c? Requesst r�t ti?n v?i ph�n trang
		/// </summary>
		/// <param name="pageNumber"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> GetAllWithdrawalRequests([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _withdrawalRequestService.GetAllAsync(pageNumber, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting withdrawal requests");
                return BadRequest(new { Message = ex.Message });
            }
        }


		/// <summary>
		/// L?y t?t c? Request r�t ti?n c?a ng??i d�ng hi?n t?i
		/// </summary>
		/// <returns></returns>
		[HttpGet("my-requests")]
        [Authorize]
        public async Task<IActionResult> GetMyWithdrawalRequests()
        {
            try
            {
                var accountId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _withdrawalRequestService.GetByAccountIdAsync(accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user's withdrawal requests");
                return BadRequest(new { Message = ex.Message });
            }
        }


		/// <summary>
		/// L?y chi ti?t m?t Request r�t ti?n theo ID
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetWithdrawalRequest(Guid id)
        {
            try
            {
                var result = await _withdrawalRequestService.GetByIdAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting withdrawal request {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }


		/// <summary>
		/// C?p nh?t tr?ng th�i c?a Request r�t ti?n
		/// </summary>
		/// <param name="id"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPut("{id}/status")]
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> UpdateWithdrawalRequestStatus(Guid id, [FromBody] UpdateWithdrawalRequestDto dto)
        {
            try
            {
                var staffId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _withdrawalRequestService.UpdateStatusAsync(id, dto, staffId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating withdrawal request status for {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }


		/// <summary>
		/// BPDV x�c nh?n ?� nh?n ti?n t? y�u c?u r�t ti?n
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpPost("{id}/confirm-received")]
        [Authorize]
        public async Task<IActionResult> ConfirmReceived(Guid id)
        {
            try
            {
                var accountId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _withdrawalRequestService.ConfirmReceivedAsync(id, accountId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming withdrawal request {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpPut("{id}/cancel")]
        [Authorize]
        public async Task<IActionResult> CancelWithdrawalRequest(Guid id)
        {
            try
            {
                var accountId = Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var result = await _withdrawalRequestService.CancelRequestAsync(id, accountId);
                if (!result)
                {
                    return BadRequest(new { Message = "Cannot cancel this withdrawal request." });
                }
                return Ok(new { Message = "Withdrawal request canceled successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error canceling withdrawal request {Id}", id);
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
} 