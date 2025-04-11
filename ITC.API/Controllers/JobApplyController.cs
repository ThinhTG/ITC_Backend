using ITC.Services.DTOs.JobApply;
using ITC.Services.JobApplicationService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITC.API.Controllers
{
	[Route("api/job-applications")]
	[ApiController]
	public class JobApplyController : ControllerBase
	{
		private readonly IJobApplyService _jobApplicationService;

		public JobApplyController(IJobApplyService jobApplicationService)
		{
			_jobApplicationService = jobApplicationService;
		}

		[HttpPost("apply")]
		[Authorize(Policy = "TalentPolicy")]
		public async Task<IActionResult> ApplyToJob([FromBody] JobApplicationCreateDto dto)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return Unauthorized();

			var interpreterId = Guid.Parse(userId);
			await _jobApplicationService.ApplyToJobAsync(interpreterId, dto);

			return Ok(new { message = "Applied successfully!" });
		}

		[HttpGet("my-applications")]
		public async Task<IActionResult> GetMyApplications([FromQuery] JobApplicationFilterDto filter)
		{
			var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userId == null)
				return Unauthorized();

			var interpreterId = Guid.Parse(userId);
			var applications = await _jobApplicationService.GetApplicationsByInterpreterIdAsync(interpreterId, filter);

			return Ok(applications);
		}

		[HttpGet("job/{jobId}")]
		public async Task<IActionResult> GetApplicationsByJobId(Guid jobId)
		{
			var applications = await _jobApplicationService.GetApplicationsByJobIdAsync(jobId);
			return Ok(applications);
		}

	}
}
