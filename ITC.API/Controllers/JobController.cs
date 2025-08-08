using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Request;
using ITC.Core.Contracts;
using ITC.Services.DTOs;
using ITC.Services.DTOs.Job;
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

		[HttpPost]
		public async Task<IActionResult> PostJob([FromBody] CreateJobPostDto dto)
		{
			try
			{
				var jobId = await _jobService.CreateJobAsync(dto);
				return Ok(new { JobId = jobId });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}

		/// <summary>
		/// Get All Job List ( cho BPDV view va Apply )
		/// </summary>
		/// <param name="search"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchJobs([FromQuery]JobFilterRequest request)
		{
			var result = await _jobService.GetAllJobsAsync(request);
			return Ok(result);
		}

		[HttpGet("by-customer/{customerId}")]
		public async Task<IActionResult> GetJobsByCustomer(Guid customerId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
		{
			var jobs = await _jobService.GetJobsByCustomerIdPaginatedAsync(customerId, pageNumber, pageSize);
			if (jobs == null || !jobs.Items.Any())
			{
				return NotFound("No jobs found for this customer.");
			}

			return Ok(jobs);
		}

		/// <summary>
		/// Lấy Job Details theo Id 
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<ActionResult<JobDetailsDto>> GetJobDetails(Guid id)
		{
			var job = await _jobService.GetJobDetailsDtoByIdAsync(id);
			if (job == null)
				return NotFound();

			return Ok(job);
		}




	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
}
