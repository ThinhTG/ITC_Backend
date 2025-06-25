using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.Services.Auth;
using ITC.Services.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ITC.API.Controllers
{
	[Route("api/auth")]
	[ApiController]
	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;
		private readonly ILogger<AuthController> _logger;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IHttpContextAccessor _httpContextAccessor;


		public AuthController(
			IAuthService authService,
			ILogger<AuthController> logger,
			UserManager<ApplicationUser> userManager, IHttpContextAccessor httpContextAccessor)
		{
			_authService = authService;
			_logger = logger;
			_userManager = userManager;
			_httpContextAccessor =httpContextAccessor;
		}

		/// <summary>
		/// Đăng kí tài khoản với thông tin người dùng ( Role = Admin,Customer,Talent )
		/// </summary>
		/// <param name="registerDto"></param>
		/// <returns></returns>
		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.RegisterAsync(registerDto);
			return Ok(result);
		}


		/// <summary>
		/// Đăng kí tài khoản với thông tin người dùng ( Role = Admin,Customer,Talent )
		/// </summary>
		/// <param name="registerDto"></param>
		/// <returns></returns>
		[HttpPost("registermb")]
		[AllowAnonymous]
		public async Task<IActionResult> RegisterMB([FromBody] RegisterDto registerDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.RegisterMBAsync(registerDto);
			return Ok(result);
		}

		/// <summary>
		/// Đăng nhập
		/// </summary>
		/// <param name="loginDto"></param>
		/// <returns></returns>
		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByEmailAsync(loginDto.UserName);
			if (user != null && user.ApprovalStatus != Core.Enum.UserApprovalStatus.Approved)
			{
				if (user.ApprovalStatus == Core.Enum.UserApprovalStatus.PendingApproval)
				{
					return Unauthorized(new { Message = "Your account is pending approval." });
				}
				if (user.ApprovalStatus == Core.Enum.UserApprovalStatus.Rejected)
				{
					return Unauthorized(new { Message = $"Your account has been rejected. Reason: {user.RejectReason}" });
				}
			}

			var result = await _authService.LoginAsync(loginDto);

			if (!result.Success)
				return Unauthorized(result);

			return Ok(result);
		}


		/// <summary>
		/// Lấy thông tin người dùng ById
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("user/{id}")]
		[AllowAnonymous]
		public async Task<IActionResult> GetById(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			// Get active subscription and determine priority
			var userIdGuid = Guid.Parse(user.Id.ToString());
			var subRepo = HttpContext.RequestServices.GetService(typeof(ITC.Repositories.Interface.IUserSubscriptionRepository)) as ITC.Repositories.Interface.IUserSubscriptionRepository;
			var activeSub = await subRepo.GetActiveSubscriptionAsync(userIdGuid);
			int priority = 0;
			if (activeSub != null && activeSub.SubscriptionPlan != null)
			{
				switch (activeSub.SubscriptionPlan.Name.ToLower())
				{
					case "partnership": priority = 1; break;
					case "premium": priority = 2; break;
					case "advance": priority = 3; break;
				}
			}

			var userWithCert = await _userManager.Users
				.Include(u => u.TranslatorCertificates)
				.FirstOrDefaultAsync(u => u.Id == user.Id);

			var response = new ITC.BusinessObject.Response.UserResponse
			{
				Id = userWithCert.Id,
				FullName = userWithCert.FullName,
				Email = userWithCert.Email,
				Gender = userWithCert.Gender,
				AvatarURL = userWithCert.AvatarUrl,
				PhoneNumber = userWithCert.PhoneNumber,
				CreateAt = userWithCert.CreatedTime.UtcDateTime,
				UpdateAt = userWithCert.LastUpdatedTime.UtcDateTime,
				AccessToken = null,
				RefreshToken = userWithCert.RefreshToken,
				Address = userWithCert.Address,
				CertificateFiles = userWithCert.CertificateFiles,
				Experience = userWithCert.Experience,
				PortraitUrl = userWithCert.PortraitUrl,
				ApprovalStatus = userWithCert.ApprovalStatus.ToString(),
				RejectReason = userWithCert.RejectReason,
				IsBoosted = false, // You can add logic to get this if needed
				Priority = priority
			};

			return Ok(response);
		}

		[HttpPost("refresh-token")]
		[AllowAnonymous] // Allow anonymous to refresh token without valid access token
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.RefreshTokenAsync(refreshTokenDto);

			if (!result.Success)
				return Unauthorized(result);

			return Ok(result);
		}

		[HttpPost("revoke-refresh-token")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> RevokeRefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
		{
			var userName = User.FindFirstValue("UserName");
			if (string.IsNullOrEmpty(userName))
			{
				_logger.LogWarning("Token revocation failed: Unable to get username from claims");
				return BadRequest(new { Message = "Invalid user information" });
			}

			var result = await _authService.LogoutAsync(userName);

			if (!result)
				return BadRequest(new { Message = "Token revocation failed" });

			return Ok(new { Message = "Token revoked successfully" });
		}


		/// <summary>
		/// xóa cứng 1 tài khoản người dùng ( yêu cầu Role Admin )
		/// </summary>
		/// <param name="id">User Id</param>
		/// <returns></returns>
		[HttpDelete("user/{id}")]
		[Authorize(Roles = "Admin")]
		public async Task<IActionResult> Delete(string id)
		{
			var user = await _userManager.FindByIdAsync(id);

			
			if (user == null)
				return NotFound(new { Message = "User not found" });

			var result = await _userManager.DeleteAsync(user);
			if (!result.Succeeded)
				return BadRequest(new { Message = "Delete failed", Errors = result.Errors });

			return Ok(new { Message = "User deleted successfully" });
		}

		//[HttpPost("google-assign-role")]
		//public async Task<IActionResult> AssignRoleForGoogleUser(AssignRoleRequest request)
		//{
		//	var user = await _userManager.FindByEmailAsync(request.Email);
		//	if (user == null)
		//		return BadRequest("User not found.");

		//	var result = await _userManager.AddToRoleAsync(user, request.Role);
		//	if (!result.Succeeded)
		//	{
		//		var errors = string.Join(", ", result.Errors.Select(e => e.Description));
		//		return BadRequest($"Role assignment failed: {errors}");
		//	}

		//	return Ok("Role assigned successfully.");
		//}

		[HttpPut("user/update")]
		[Authorize]
		public async Task<IActionResult> UpdateUser([FromBody] UserUpdateDto updateDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByIdAsync(updateDto.Id);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			user.FullName = updateDto.FullName ?? user.FullName;
			user.AvatarUrl = updateDto.AvatarUrl ?? user.AvatarUrl;
			user.Gender = updateDto.Gender ?? user.Gender;
			user.Address = updateDto.Address ?? user.Address;
			user.LastUpdatedTime = DateTimeOffset.UtcNow;

			var result = await _userManager.UpdateAsync(user);
			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				return BadRequest(new { Message = "Update failed", Errors = errors });
			}

			return Ok(new { Message = "User updated successfully" });
		}

		/// <summary>
		/// Google Login
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("google-login")]
		[AllowAnonymous]
		public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
		{
			return Ok(await _authService.LoginGoogle(request));
		}

		/// <summary>
		/// Assign Role vào nếu login GG chưa có Role ( User chọn Role)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("google/assign-role")]
		public async Task<IActionResult> AssignRoleToGoogleUser([FromBody] AssignRoleRequest request)
		{
			var result = await _authService.AssignRoleToGoogleUserAsync(request.Email, request.Role);
			if (!result.Success) return BadRequest(result);
			return Ok(result);
		}

		//[HttpGet("confirm-email")]
		//public async Task<IActionResult> ConfirmEmail(string userId, string token)
		//{
		//	var user = await _userManager.FindByIdAsync(userId);
		//	if (user == null) return NotFound("Người dùng không tồn tại.");

		//	var result = await _userManager.ConfirmEmailAsync(user, token);
		//	if (result.Succeeded)
		//		return Ok("Email đã được xác thực thành công.");

		//	return BadRequest("Xác thực email thất bại.");
		//}

		[HttpGet("confirm-email")]
		[HttpPost("confirm-email")]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			bool isConfirmed = await _authService.ConfirmEmailAsync(userId, token);
			if (isConfirmed)
				return Redirect("https://inter-trans-connect.web.app/welcome");   // deploy sửa lại
			return BadRequest("Xác nhận email thất bại.");
		}


		[HttpPut("bank-account")]
		public async Task<IActionResult> UpdateBankAccount([FromBody] UpdateBankAccountRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (userIdClaim == null) return Unauthorized();

			var userId = Guid.Parse(userIdClaim);
			var success = await _authService.UpdateBankAccountAsync(userId, request);

			return success ? Ok(new { message = "Bank account updated successfully" })
						   : BadRequest(new { message = "Failed to update bank account" });
		}



	}
}
