using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
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

		[HttpPost]
		public async Task<IActionResult> PostJob([FromBody] CreateJobPostDto dto)
		{
			var jobId = await _jobService.CreateJobAsync(dto);
			return Ok(new { JobId = jobId });
		}

		/// <summary>
		/// Get All Job List ( cho BPDV view va Apply )
		/// </summary>
		/// <param name="search"></param>
		/// <param name="page"></param>
		/// <param name="pageSize"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
		{
			var result = await _jobService.GetAllJobsAsync(search, page, pageSize);
			return Ok(result);
		}


		[HttpGet("by-customer/{customerId}")]
		public async Task<IActionResult> GetJobsByCustomer(Guid customerId)
		{
			var jobs = await _jobService.GetJobsByCustomerIdAsync(customerId);
			if (jobs == null || !jobs.Any())
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
		public async Task<IActionResult> GetJobById(Guid id)
		{
			var job = await _jobService.GetJobDetailsByIdAsync(id);
			if (job == null)
				return NotFound(new { message = "Job not found." });

			return Ok(job);
		}



	}                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                  
}
