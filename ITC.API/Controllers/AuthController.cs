using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.Services.Auth;
using ITC.Services.DTOs.Auth;
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

			var result = await _authService.LoginAsync(loginDto);

			if (!result.Success)
				return Unauthorized(result);

			return Ok(result);
		}


		/// <summary>
		/// Lấy thông tin người dùng hiện tại
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("user/{id}")]
		[Authorize]
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

		[HttpPost("google-assign-role")]
		public async Task<IActionResult> AssignRoleForGoogleUser(AssignRoleRequest request)
		{
			var user = await _userManager.FindByEmailAsync(request.Email);
			if (user == null)
				return BadRequest("User not found.");

			var result = await _userManager.AddToRoleAsync(user, request.Role);
			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				return BadRequest($"Role assignment failed: {errors}");
			}

			return Ok("Role assigned successfully.");
		}

	}
}
