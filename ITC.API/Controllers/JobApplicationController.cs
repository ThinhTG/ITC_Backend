using ITC.Services.DTOs.JobApply;
using ITC.Services.JobApplyService;
using ITC.Services.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ITC.BusinessObject.Identity;
using System.Security.Claims;
using System.IO;

namespace ITC.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class JobApplicationController : ControllerBase
	{
		private readonly IJobApplicationService _service;
		private readonly ITranslatorCertificateService _certificateService;
		private readonly UserManager<ApplicationUser> _userManager;

		public JobApplicationController(
			IJobApplicationService service,
			ITranslatorCertificateService certificateService,
			UserManager<ApplicationUser> userManager)
		{
			_service = service;
			_certificateService = certificateService;
			_userManager = userManager;
		}


		/// <summary>
		/// Apply to a Job
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		[Authorize]
		public async Task<IActionResult> ApplyToJob([FromBody] JobApplicationDto dto)
		{
			try
			{
				// Lấy thông tin user hiện tại
				var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
				if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
				{
					return Unauthorized("Invalid user identifier.");
				}
				
				var user = await _userManager.FindByIdAsync(userId.ToString());
				if (user == null)
				{
					return Unauthorized("User not found.");
				}
				
				// Kiểm tra nếu là Talent thì phải có certificate được duyệt
				var userRoles = await _userManager.GetRolesAsync(user);
				if (userRoles.Contains("Talent"))
				{
					var certificates = await _certificateService.GetCertificatesByUserIdAsync(userId);
					
					if (!certificates.Any())
					{
						return BadRequest(new { Message = "Hãy cập nhật certificate trước khi apply job." });
					}
					
					var approvedCertificate = certificates.FirstOrDefault(c => c.Status == Core.Enum.CertificateStatus.Approved);
					if (approvedCertificate == null)
					{
						return BadRequest(new { Message = "Hãy đợi trong vòng 24 tiếng để certificate được duyệt." });
					}
				}

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
