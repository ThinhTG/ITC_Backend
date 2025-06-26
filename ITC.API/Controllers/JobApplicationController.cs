using ITC.Services.DTOs.JobApply;
using ITC.Services.JobApplyService;
using ITC.Services.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using ITC.BusinessObject.Identity;
using System.Security.Claims;
using ITC.Core.Base;
using ITC.Core.Constants;

namespace ITC.API.Controllers
{
	[ApiController]
    [Route("api/applications")]
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
				// L?y thông tin user hi?n t?i
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

				// Ki?m tra n?u là Talent thì ph?i có certificate ???c duy?t
				var userRoles = await _userManager.GetRolesAsync(user);
				if (userRoles.Contains("Talent"))
				{
					var certificates = await _certificateService.GetCertificatesByUserIdAsync(userId);

					if (!certificates.Any())
					{
						return Ok(new BaseResponse<string>(
						StatusCodeHelper.NotFound,
						ResponseCodeConstants.NOT_FOUND,
						"Update your certifications before applying for jobs."));
					}

					var approvedCertificate = certificates.FirstOrDefault(c => c.Status == Core.Enum.CertificateStatus.Approved);
					if (approvedCertificate == null)
					{
						return Ok(new BaseResponse<string>(
						StatusCodeHelper.NotFound,
						ResponseCodeConstants.NOT_FOUND,
						"Please wait 24 hours for the certificate to be approved."));
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

		[HttpGet("job/{jobId}")]
		public async Task<IActionResult> GetApplications(Guid jobId)
		{
			var result = await _service.GetApplicationsForJobWithDetailsAsync(jobId);
			return Ok(result);
		}

        [HttpPost("select")]
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

        [HttpPost("reject")]
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

		[HttpGet("interpreter/{interpreterId}")]
		public async Task<IActionResult> GetApplicationsByInterpreter(Guid interpreterId)
		{
			var result = await _service.GetApplicationsByInterpreterId(interpreterId);
			return Ok(result);
		}
	}
}