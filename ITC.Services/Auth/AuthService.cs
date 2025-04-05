using ITC.BusinessObject.Identity;
using ITC.Core.Base;
using ITC.Services.DTOs;
using ITC.Services.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly ILogger<AuthService> _logger;
		private readonly double _refreshTokenExpiryDays;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			ITokenService tokenService,
			ILogger<AuthService> logger,
			IConfiguration configuration)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_logger = logger;

			var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
			_refreshTokenExpiryDays = jwtSettings?.RefreshTokenExpirationDays ?? 7;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
		{
			_logger.LogInformation("Starting user registration process for {Email}", registerDto.Email);

			// Check if user already exists
			var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUser != null)
			{
				_logger.LogWarning("Registration failed: Email {Email} is already in use", registerDto.Email);
				return new AuthResponseDto
				{
					Success = false,
					Message = "Email is already in use."
				};
			}

			// Create new user
			var user = new ApplicationUser
			{
				UserName = registerDto.UserName,
				Email = registerDto.Email,
				// Add other required properties here
			};

			var result = await _userManager.CreateAsync(user, registerDto.Password);

			if (!result.Succeeded)
			{
				var errors = string.Join(", ", result.Errors.Select(e => e.Description));
				_logger.LogWarning("Registration failed for {Email}: {Errors}", registerDto.Email, errors);
				return new AuthResponseDto
				{
					Success = false,
					Message = $"Registration failed: {errors}"
				};
			}

			// Assign default role if needed
			await _userManager.AddToRoleAsync(user, "Customer");

			_logger.LogInformation("User {Email} registered successfully", registerDto.Email);

			// Generate tokens
			var accessToken = await _tokenService.GenerateToken(user);
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Save refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);

			return new AuthResponseDto
			{
				Success = true,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				Message = "User registered successfully"
			};
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
		{
			_logger.LogInformation("Login attempt for user: {UserName}", loginDto.UserName);

			var user = await _userManager.FindByNameAsync(loginDto.UserName)
					?? await _userManager.FindByEmailAsync(loginDto.UserName);

			if (user == null)
			{
				_logger.LogWarning("Login failed: User {UserName} not found", loginDto.UserName);
				return new AuthResponseDto
				{
					Success = false,
					Message = "Invalid username or password."
				};
			}

			var isPasswordValid = await _userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!isPasswordValid)
			{
				_logger.LogWarning("Login failed: Invalid password for user {UserName}", loginDto.UserName);
				return new AuthResponseDto
				{
					Success = false,
					Message = "Invalid username or password."
				};
			}

			// Generate tokens
			var accessToken = await _tokenService.GenerateToken(user);
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Save refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);

			_logger.LogInformation("User {UserName} logged in successfully", loginDto.UserName);

			return new AuthResponseDto
			{
				Success = true,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				Message = "Login successful"
			};
		}

		public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
		{
			_logger.LogInformation("Processing refresh token request");

			// Find the user based on the token
			var user = await FindUserByRefreshTokenAsync(refreshTokenDto.RefreshToken);
			if (user == null)
			{
				_logger.LogWarning("Refresh token is invalid");
				return new AuthResponseDto
				{
					Success = false,
					Message = "Invalid refresh token."
				};
			}

			// Check if refresh token is expired
			if (user.RefreshTokenExpiryTime <= DateTime.Now)
			{
				_logger.LogWarning("Refresh token expired for user {UserName}", user.UserName);
				return new AuthResponseDto
				{
					Success = false,
					Message = "Refresh token has expired."
				};
			}

			// Generate new tokens
			var newAccessToken = await _tokenService.GenerateToken(user);
			var newRefreshToken = _tokenService.GenerateRefreshToken();

			// Update refresh token
			user.RefreshToken = newRefreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);

			_logger.LogInformation("Tokens refreshed successfully for user {UserName}", user.UserName);

			return new AuthResponseDto
			{
				Success = true,
				AccessToken = newAccessToken,
				RefreshToken = newRefreshToken,
				Message = "Tokens refreshed successfully"
			};
		}

		public async Task<bool> LogoutAsync(string userName)
		{
			_logger.LogInformation("Logout attempt for user: {UserName}", userName);

			var user = await _userManager.FindByNameAsync(userName);
			if (user == null)
			{
				_logger.LogWarning("Logout failed: User {UserName} not found", userName);
				return false;
			}

			// Clear refresh token
			user.RefreshToken = null;
			user.RefreshTokenExpiryTime = DateTime.MinValue;
			var result = await _userManager.UpdateAsync(user);

			if (!result.Succeeded)
			{
				_logger.LogWarning("Failed to update user during logout: {Errors}",
					string.Join(", ", result.Errors.Select(e => e.Description)));
				return false;
			}

			_logger.LogInformation("User {UserName} logged out successfully", userName);
			return true;
		}

		private async Task<ApplicationUser> FindUserByRefreshTokenAsync(string refreshToken)
		{
			var users = _userManager.Users.Where(u => u.RefreshToken == refreshToken).ToList();
			if (!users.Any())
				return null;

			return users.First();
		}
	}
}

