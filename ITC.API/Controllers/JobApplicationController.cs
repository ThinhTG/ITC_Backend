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


		/// <summary>
		/// BPDV ung tuyen vao 1 Job
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
		/// Lay toan bo Apply cua 1 Job nao do
		/// </summary>
		/// <param name="jobId">nhap JobId vao de xem cac thanh vien ung tuyen</param>
		/// <returns></returns>
		[HttpGet("{jobId}/applications")]
		public async Task<IActionResult> GetApplications(Guid jobId)
		{
			var result = await _service.GetApplicationsForJobAsync(jobId);
			return Ok(result);
		}


		/// <summary>
		/// Sau khi xem xet cac apply thi duoc chon 1 nguoi de lam job do
		/// </summary>
		/// <param name="jobId">nhap jobId</param>
		/// <param name="intrepreterId">nhap Id cua nguoi Ung tuyen duoc chon</param>
		/// <returns></returns>
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
