using AutoMapper;
using Google.Apis.Auth;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.BusinessObject.Response;
using ITC.Core.Base;
using ITC.Services.DTOs.Auth;
using ITC.Services.TokenService;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace ITC.Services.Auth
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly ITokenService _tokenService;
		private readonly ILogger<AuthService> _logger;
		private readonly double _refreshTokenExpiryDays;
		private readonly IMapper _mapper;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			ITokenService tokenService,
			ILogger<AuthService> logger,
			IConfiguration configuration,
			IMapper mapper)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_logger = logger;

			var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>();
			_refreshTokenExpiryDays = jwtSettings?.RefreshTokenExpirationDays ?? 7;
			_mapper = mapper;
		}

		public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
		{
			// Check if user already exists
			var existingUser = await _userManager.FindByEmailAsync(registerDto.Email);
			if (existingUser != null)
			{
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
				PhoneNumber = registerDto.PhoneNumber,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				FullName = registerDto.UserName,
				Address = registerDto.Address
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

			if (registerDto.Role.Equals("Customer"))
			{
				await _userManager.AddToRoleAsync(user, "Customer");
			}
			else if (registerDto.Role.Equals("Talent"))
			{
				await _userManager.AddToRoleAsync(user, "Talent");
			}
			else
			{
				await _userManager.AddToRoleAsync(user, "Admin");
			}

			var refreshToken = _tokenService.GenerateRefreshToken();
			// Save refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);
			return new AuthResponseDto
			{
				Success = true,
				Message = "User registered successfully"
			};
		}

		public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
		{

			var user = await _userManager.FindByNameAsync(loginDto.UserName)
					?? await _userManager.FindByEmailAsync(loginDto.UserName);

			if (user == null)
			{
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
			var userRes = _mapper.Map<ApplicationUser, UserResponse>(user);

			return new AuthResponseDto
			{
				Success = true,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				Message = "Login successful",
				User = userRes
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

		public async Task<UserResponse> LoginGoogle(GoogleLoginRequest request)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token)
						  ?? throw new Exception("Invalid Google token.");

			string email = payload.Email;
			string name = payload.Name;
			string googleId = payload.Subject;

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				user = new ApplicationUser
				{
					Email = email,
					UserName = googleId,
					FullName = payload.Name ?? "Unknown",
					Gender = "Not Specified",
					PhoneNumber = "Unknown",
					Address = "Not Provided",
					CreatedTime = DateTime.UtcNow,
					LastUpdatedTime = DateTime.UtcNow
				};

				var createResult = await _userManager.CreateAsync(user);
				if (!createResult.Succeeded)
				{
					var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
					throw new Exception($"User creation failed: {errors}");
				}
				var roleResult = await _userManager.AddToRoleAsync(user, "User");
				if (!roleResult.Succeeded)
				{
					var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
					_logger.LogError("Role assignment failed: {errors}", errors);
					throw new Exception($"Role assignment failed: {errors}");
				}
			}

			// Generate access token
			var token = await _tokenService.GenerateToken(user);

			// Generate refresh token
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Hash the refresh token and store it in the database or override the existing refresh token
			using var sha256 = SHA256.Create();
			var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
			user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
			user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2);


			var updateResult = await _userManager.UpdateAsync(user);
			if (!updateResult.Succeeded)
			{
				var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
				_logger.LogError("Failed to update user: {errors}", errors);
				throw new Exception($"Failed to update user: {errors}");
			}

			var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
			userResponse.AccessToken = token;
			userResponse.RefreshToken = refreshToken;
			userResponse.Address = user.Address;

			return userResponse;
		}
	}
}

