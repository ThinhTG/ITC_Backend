using ITC.BusinessObject.Request;
using ITC.Services.User;
using ITC.Services.Certificate;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ITC.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITranslatorCertificateService _certificateService;

        public AdminController(
            IUserService userService,
            ITranslatorCertificateService certificateService)
        {
            _userService = userService;
            _certificateService = certificateService;
        }

		/// <summary>
		/// Lấy tất cẩ cácBPDV (Translator) đang chờ phê duyệt.
		/// </summary>
		/// <returns></returns>
		[HttpGet("pending-approvals")]
        public async Task<IActionResult> GetPendingApprovalUsers()
        {
            var users = await _userService.GetPendingApprovalUsersAsync();
            return Ok(users);
        }


		/// <summary>
		/// Phê duyệt cho 1 BPDV (Translator).
		/// </summary>
		/// <param name="userId"></param>
		/// <returns></returns>
		[HttpPost("approve-user/{userId}")]
        public async Task<IActionResult> ApproveUser(Guid userId)
        {
            var result = await _userService.ApproveUserAsync(userId);
            if (!result)
            {
                return NotFound(new { Message = "User not found or approval failed." });
            }
            return Ok(new { Message = "User approved successfully." });
        }

		/// <summary>
		/// Từ chối phê duyệt cho 1 BPDV (Translator).
		/// </summary>
		/// <param name="userId"></param>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("reject-user/{userId}")]
        public async Task<IActionResult> RejectUser(Guid userId, [FromBody] RejectUserRequest request)
        {
            var result = await _userService.RejectUserAsync(userId, request);
            if (!result)
            {
                return NotFound(new { Message = "User not found or rejection failed." });
            }
            return Ok(new { Message = "User rejected successfully." });
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserDetail(Guid userId)
        {
            var users = await _userService.GetPendingApprovalUsersAsync();
            var user = users.FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return NotFound(new { Message = "User not found." });
            }
            return Ok(user);
        }

        // Certificate management
        [HttpGet("pending-certificates")]
        public async Task<IActionResult> GetPendingCertificates()
        {
            var certificates = await _certificateService.GetPendingCertificatesAsync();
            return Ok(certificates);
        }

        [HttpPost("approve-certificate/{certificateId}")]
        public async Task<IActionResult> ApproveCertificate(Guid certificateId)
        {
            var result = await _certificateService.ApproveCertificateAsync(certificateId);
            if (!result)
            {
                return NotFound(new { Message = "Certificate not found or approval failed." });
            }
            return Ok(new { Message = "Certificate approved successfully." });
        }

        [HttpPost("reject-certificate/{certificateId}")]
        public async Task<IActionResult> RejectCertificate(Guid certificateId, [FromBody] RejectUserRequest request)
        {
            var result = await _certificateService.RejectCertificateAsync(certificateId, request.Reason);
            if (!result)
            {
                return NotFound(new { Message = "Certificate not found or rejection failed." });
            }
            return Ok(new { Message = "Certificate rejected successfully." });
        }
    }
} 