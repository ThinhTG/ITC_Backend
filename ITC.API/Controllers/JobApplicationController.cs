using ITC.Services.DTOs.JobApply;
using ITC.Services.JobApplyService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JobApplicationController : ControllerBase
	{
		private readonly IJobApplicationService _service;

		public JobApplicationController(IJobApplicationService service)
		{
			_service = service;
		}

		[HttpPost]
		public async Task<IActionResult> ApplyToJob([FromBody] JobApplicationDto dto)
		{
			try
			{
				await _service.ApplyAsync(dto);
				return Ok("Application submitted successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		[HttpGet("{jobId}/applications")]
		public async Task<IActionResult> GetApplications(Guid jobId)
		{
			var result = await _service.GetApplicationsForJobAsync(jobId);
			return Ok(result);
		}

		[HttpPost("{jobId}/select")]
		public async Task<IActionResult> SelectInterpreter(Guid jobId, [FromBody] Guid intrepreterId)
		{
			try
			{
				await _service.SelectInterpreterAsync(jobId, intrepreterId);
				return Ok("Interpreter selected successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
	}

}
