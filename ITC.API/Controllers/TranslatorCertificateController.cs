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

		[HttpGet("{userId}")]
		public async Task<IActionResult> Get(Guid userId)
		{
			var result = await _service.GetByUserIdAsync(userId);
			if (result == null)
			{
				return NotFound(new
				{
					message = "The interpreter has not updated their certificate information yet."
				});
			}

			return Ok(result);
		}


		[HttpPost("{userId}")]
		public async Task<IActionResult> AddOrUpdate(Guid userId, [FromBody] TranslatorCertificateCreateUpdateDto dto)
		{
			await _service.AddOrUpdateAsync(userId, dto);
			return Ok(new
			{
				message = "Certificate information has been successfully added or updated."
			});
		}


		[HttpDelete("{userId}")]
		public async Task<IActionResult> Delete(Guid userId)
		{
			await _service.DeleteAsync(userId);
			return Ok(new
			{
				message = "Certificate information has been successfully deleted."
			});
		}

	}

}
