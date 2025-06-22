using ITC.Core.Contracts;
using ITC.Services.Certificate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class TranslatorCertificateController : ControllerBase
	{
		private readonly ITranslatorCertificateService _service;

		public TranslatorCertificateController(ITranslatorCertificateService service)
		{
			_service = service;
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
	}
}
