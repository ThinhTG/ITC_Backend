using ITC.Services.DTOs.JobApply;
using ITC.Services.JobApplyService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IO;

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


		/// <summary>
		/// Apply to a Job
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
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

		/// <summary>
		/// Lay Toan Bo Apply cua 1 Job (bao gồm thông tin file upload của customer)
		/// </summary>
		/// <param name="jobId"></param>
		/// <returns></returns>
		[HttpGet("{jobId}/applications")]
		public async Task<IActionResult> GetApplications(Guid jobId)
		{
			var result = await _service.GetApplicationsForJobWithDetailsAsync(jobId);
			return Ok(result);
		}


		/// <summary>
		/// Chon BPD cho 1 Job
		/// </summary>
		/// <param name="jobId">Nhap JobID</param>
		/// <param name="intrepreterId">Nhap ID của BPDV</param>
		/// <returns></returns>
		[HttpPost("/select")]
		public async Task<IActionResult> SelectInterpreter([FromBody]SelectInterRequest selectInterRequest)
		{
			try
			{
				await _service.SelectInterpreterAsync(selectInterRequest);
				return Ok("Interpreter selected successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// Từ chối BPDV cho 1 Job
		/// </summary>
		/// <param name="rejectRequest">Chứa JobId và InterpreterId</param>
		/// <returns></returns>
		[HttpPost("/reject")]
		public async Task<IActionResult> RejectInterpreter([FromBody]SelectInterRequest rejectRequest)
		{
			try
			{
				await _service.RejectInterpreterAsync(rejectRequest);
				return Ok("Interpreter rejected successfully");
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}

		/// <summary>
		/// get all Apply cua 1 interpreter 
		/// </summary>
		/// <param name="interpreterId"></param>
		/// <returns></returns>
		[HttpGet("interpreter/{interpreterId}")]
		public async Task<IActionResult> GetApplicationsByInterpreter(Guid interpreterId)
		{
			var result = await _service.GetApplicationsByInterpreterId(interpreterId);
			return Ok(result);
		}

	}

}
