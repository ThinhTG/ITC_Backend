using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.Services.DTOs;
using ITC.Services.JobService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITC.API.Controllers
{

	[ApiController]
	[Route("api/job")]

	public class JobController : ControllerBase
	{
		private readonly IJobService _jobService;

		public JobController(IJobService jobService)
		{
			_jobService = jobService;
		}

		[HttpPost("post")]
		public async Task<IActionResult> PostJob([FromForm] CreateJobRequest jobDto)
		{
			try
			{
				var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (userId == null)
					return Unauthorized();

				var customerId = Guid.Parse(userId);

				await _jobService.PostJobAsync(jobDto, customerId);
				return Ok(new { success = true, message = "Job posted successfully!" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get All Job List
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAvailableJobs()
		{
			var jobs = await _jobService.GetAllAvailableJobsAsync();
			return Ok(jobs);
		}


		[HttpGet("by-customer/{customerId}")]
		public async Task<IActionResult> GetJobsByCustomer(Guid customerId)
		{
			var jobs = await _jobService.GetJobsByCustomerIdAsync(customerId);
			if (jobs == null || !jobs.Any())
			{
				return NotFound("No jobs found for this customer.");
			}

			return Ok(jobs); // bạn có thể chuyển sang DTO nếu muốn
		}



	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
}
