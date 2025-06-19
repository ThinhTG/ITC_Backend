using ITC.BusinessObject.Request;
using ITC.Services.JobWork;
using ITC.Services.JobService;
using ITC.Services.JobApplyService;
using ITC.Core.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class JobWorkController : ControllerBase
	{
		private readonly IJobWorkService _svc;
		private readonly IJobService _jobService;
		private readonly IJobApplicationService _jobApplicationService;

		public JobWorkController(IJobWorkService svc, IJobService jobService, IJobApplicationService jobApplicationService)
		{
			_svc = svc;
			_jobService = jobService;
			_jobApplicationService = jobApplicationService;
		}

		/// <summary>
		/// BPDV bắt đầu làm việc - chuyển trạng thái từ Paid sang InProgress
		/// </summary>
		/// <param name="interpreterId"></param>
		/// <param name="jobId"></param>
		/// <returns></returns>
		[HttpPost("{jobId}/start-work")]
		[Authorize(Roles = "Talent")]
		public async Task<IActionResult> StartWork(Guid interpreterId, Guid jobId)
		{
			await _svc.StartWorkAsync(jobId, interpreterId);
			return Ok(new { message = "Đã bắt đầu làm việc." });
		}

		/// <summary>
		/// API submit file kết quả công việc của Biên dịch viên   // hoặc đánh dấu hoàn thành công việc của phiên dịch viên
		/// </summary>
		/// <param name="interpreterId"></param>
		/// <param name="jobId"></param>
		/// <param name="body"></param>
		/// <returns></returns>
		[HttpPost("{jobId}/submit")]
		[Authorize(Roles = "Talent")]
		public async Task<IActionResult> Submit(Guid interpreterId, Guid jobId, SubmitWorkRequest body)
		{
			//var interpreterId = User.();       // extension lấy từ Claim
			await _svc.SubmitWorkAsync(jobId, interpreterId, body.ResultFileUrl);
			return Ok(new { message = "Đã nộp kết quả, chờ khách xác nhận." });
		}

		/// <summary>
		/// khách hàng xác nhận công việc đã hoàn thành và chuyển lương vào ví BPDV
		/// </summary>
		/// <param name="jobId">nhập Job Id</param>
		/// <param name="interpreterId">Nhập BPDV Id</param>
		/// <returns></returns>
		[HttpPost("{jobId}/confirm-completion/{interpreterId}")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> ConfirmCompletion(Guid jobId, Guid customerId)
		{
			await _svc.ConfirmCompletionAsync(jobId, customerId);
			return Ok(new { message = "Đã đánh dấu hoàn thành công việc." });
		}

		/// <summary>
		/// TEMPORARY: Bypass payment for testing - chuyển trạng thái từ AwaitingPayment sang Paid
		/// </summary>
		/// <param name="interpreterId"></param>
		/// <param name="jobId"></param>
		/// <returns></returns>
		[HttpPost("{jobId}/bypass-payment")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> BypassPayment(Guid interpreterId, Guid jobId)
		{
			try
			{
				var job = await _jobService.GetJobDetailsDtoByIdAsync(jobId);
				if (job == null)
				{
					return NotFound(new { message = "Job not found" });
				}

				var applications = await _jobApplicationService.GetApplicationsForJobAsync(jobId);
				var application = applications.FirstOrDefault(a => a.InterpreterId == interpreterId);
				
				if (application == null)
				{
					return NotFound(new { message = "Interpreter application not found" });
				}

				if (application.WorkStatus != (int)InterpreterWorkStatus.AwaitingPayment)
				{
					return BadRequest(new { message = $"Interpreter is not in awaiting payment status. Current status: {application.WorkStatus}" });
				}

				// Bypass payment - update status directly
				application.WorkStatus = (int)InterpreterWorkStatus.Paid;
				application.IsPaid = true;
				application.IndividualFee = 100000; // Default amount for testing
				application.PaidAt = DateTimeOffset.UtcNow;
				application.LastUpdatedAt = DateTimeOffset.UtcNow;

				// Save changes
				await _jobApplicationService.SaveChangesAsync();

				return Ok(new { message = "Payment bypassed successfully. Interpreter can now start work." });
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message, details = ex.StackTrace });
			}
		}

		/// <summary>
		/// Debug endpoint để kiểm tra trạng thái job và applications
		/// </summary>
		/// <param name="jobId"></param>
		/// <returns></returns>
		[HttpGet("{jobId}/debug")]
		[Authorize]
		public async Task<IActionResult> DebugJob(Guid jobId)
		{
			try
			{
				var job = await _jobService.GetJobDetailsDtoByIdAsync(jobId);
				if (job == null)
				{
					return NotFound(new { message = "Job not found" });
				}

				var applications = await _jobApplicationService.GetApplicationsForJobAsync(jobId);
				
				return Ok(new
				{
					JobId = job.Id,
					JobTitle = job.JobTitle,
					JobStatus = job.Status,
					RequiredHires = job.RequiredHires,
					CurrentHires = job.CurrentHires,
					Applications = applications.Select(a => new
					{
						ApplicationId = a.Id,
						InterpreterId = a.InterpreterId,
						ApplicationStatus = a.ApplicationStatus,
						WorkStatus = a.WorkStatus,
						IsPaid = a.IsPaid,
						IndividualFee = a.IndividualFee,
						StartedAt = a.StartedAt,
						CompletedAt = a.CompletedAt
					}).ToList()
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message, details = ex.StackTrace });
			}
		}
	}
}
