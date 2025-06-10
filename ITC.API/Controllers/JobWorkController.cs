using ITC.BusinessObject.Request;
using ITC.Services.JobWork;
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

		public JobWorkController(IJobWorkService svc)
		{
			_svc = svc;
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
		public async Task<IActionResult> ConfirmCompletion(Guid jobId, Guid interpreterId)
		{
			await _svc.ConfirmCompletionAsync(jobId, interpreterId);
			return Ok(new { message = "Đã đánh dấu hoàn thành công việc." });
		}

	}
}
