using ITC.BusinessObject.Identity;
using ITC.Services.Auth;
using ITC.Services.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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

		public AuthController(
			IAuthService authService,
			ILogger<AuthController> logger,
			UserManager<ApplicationUser> userManager)
		{
			_authService = authService;
			_logger = logger;
			_userManager = userManager;
		}

		[HttpPost("register")]
		[AllowAnonymous]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.RegisterAsync(registerDto);
			return Ok(result);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _authService.LoginAsync(loginDto);

			if (!result.Success)
				return Unauthorized(result);

			return Ok(result);
		}

		[HttpGet("user/{id}")]
		[Authorize(Policy = "UserPolicy")]
		public async Task<IActionResult> GetById(string id)
		{
			var user = await _userManager.FindByIdAsync(id);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			return Ok(new
			{
				Id = user.Id,
				UserName = user.UserName,
				Email = user.Email,
				// Add other properties you want to return
			});
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

		[HttpGet("confirm-email")]
		[AllowAnonymous]
		public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, [FromQuery] string token)
		{
			if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(token))
				return BadRequest(new { Message = "Invalid email confirmation parameters" });

			var user = await _userManager.FindByIdAsync(userId);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			token = System.Web.HttpUtility.UrlDecode(token);
			var result = await _userManager.ConfirmEmailAsync(user, token);

			if (result.Succeeded)
				return Redirect("http://localhost:3000/welcome"); // Update with your frontend URL

			return BadRequest(new { Message = "Email confirmation failed", Errors = result.Errors });
		}

		[HttpPost("resend-confirm-email")]
		[AllowAnonymous]
		public async Task<IActionResult> ResendConfirmEmail([FromBody] ResendEmailDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
				return BadRequest(new { Message = "Email is required" });

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return NotFound(new { Message = "User not found" });

			if (user.EmailConfirmed)
				return BadRequest(new { Message = "Email already confirmed" });

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
			// Here you would send the email with the token
			// You'll need to implement an email service for this

			return Ok(new { Message = "Confirmation email resent successfully" });
		}

		[HttpPost("forgot-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto request)
		{
			if (string.IsNullOrWhiteSpace(request.Email))
				return BadRequest(new { Message = "Email is required" });

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return Ok(new { Message = "If your email is registered, you will receive a password reset link" });

			var token = await _userManager.GeneratePasswordResetTokenAsync(user);
			// Here you would send the email with the token
			// You'll need to implement an email service for this

			return Ok(new { Message = "Password reset email sent successfully" });
		}

		[HttpPost("reset-password")]
		[AllowAnonymous]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest(new { Message = "User not found" });

			var token = System.Web.HttpUtility.UrlDecode(request.Token);
			var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

			if (!result.Succeeded)
				return BadRequest(new { Message = "Password reset failed", Errors = result.Errors });

			return Ok(new { Message = "Password has been reset successfully" });
		}

		//[HttpPost("update-avatar")]
		//[Authorize]
		//public async Task<IActionResult> UpdateAvatar([FromBody] UpdateAvatarDto model)
		//{
		//	var user = await _userManager.FindByIdAsync(model.UserId);
		//	if (user == null)
		//		return NotFound(new { Message = "User not found" });

		//	// Update avatar URL - assuming you have this property in your ApplicationUser class
		//	user.AvatarUrl = model.AvatarUrl;

		//	var result = await _userManager.UpdateAsync(user);
		//	if (!result.Succeeded)
		//		return BadRequest(new { Message = "Avatar update failed", Errors = result.Errors });

		//	return Ok(new { Message = "Avatar updated successfully" });
		//}
	}
}
