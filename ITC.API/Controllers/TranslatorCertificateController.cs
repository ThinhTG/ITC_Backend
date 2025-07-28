using ITC.Core.Contracts;
using ITC.Services.Certificate;
using ITC.Services.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ITC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TranslatorCertificateController : ControllerBase
	{
		private readonly ITranslatorCertificateService _service;
		private readonly IUserService _userService;

		public TranslatorCertificateController(ITranslatorCertificateService service, IUserService userService )
		{
			_service = service;
			_userService = userService;
		}

		[HttpGet("user/{userId}")]
		public async Task<IActionResult> GetByUserId(Guid userId)
		{
			var result = await _service.GetByUserIdAsync(userId);
			if (result == null || !result.Any())
			{
				return NotFound(new
				{
					message = "The interpreter has not updated their certificate information yet."
				});
			}

			return Ok(result);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			try
			{
				var result = await _service.GetByIdAsync(id);
				return Ok(result);
			}
			catch (KeyNotFoundException)
			{
				return NotFound(new
				{
					message = "Certificate not found."
				});
			}
		}

		[HttpGet("user-approval-stats")]
		public async Task<IActionResult> GetUserApprovalStatusStats()
		{
			var stats = await _userService.GetUserApprovalStatusStatsAsync();
			if ( stats.Approved == 0 && stats.PendingApproval == 0 && stats.Rejected == 0)
			{
				return Ok("No Certificate");
			}
			else if (stats.Approved > 0)
			{
				return Ok("Approved");
			}
			return Ok("PandingApproval");
		}


		/// <summary>
		/// Cập nhật thông tin chứng chỉ cho BPDV (Translator) sau khi cập nhật thì đợi Admin Approve
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost("user/{userId}")]
		public async Task<IActionResult> Add(Guid userId, [FromBody] TranslatorCertificateCreateUpdateDto dto)
		{
			var result = await _service.AddAsync(userId, dto);
			return Ok(new
			{
				message = "Certificate information has been successfully added.",
				data = result
			});
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(Guid id, [FromBody] TranslatorCertificateCreateUpdateDto dto)
		{
			try
			{
				await _service.UpdateAsync(id, dto);
				return Ok(new
				{
					message = "Certificate information has been successfully updated."
				});
			}
			catch (KeyNotFoundException)
			{
				return NotFound(new
				{
					message = "Certificate not found."
				});
			}
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(Guid id)
		{
			try
			{
				await _service.DeleteAsync(id);
				return Ok(new
				{
					message = "Certificate information has been successfully deleted."
				});
			}
			catch (KeyNotFoundException)
			{
				return NotFound(new
				{
					message = "Certificate not found."
				});
			}
		}

		/// <summary>
		/// api get Status của chứng chỉ của người dùng
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpGet("status/{userId}")]
		public async Task<IActionResult> GetCertificateStatus(Guid userId)
		{
			var status = await _service.GetCertificateStatusAsync(userId);
			return Ok(status);
		}
	}
}
