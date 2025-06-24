using AutoMapper;
using Google.Apis.Auth;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.BusinessObject.Response;
using ITC.Core.Base;
using ITC.Core.Utils;
using ITC.Core.Enum;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.Auth;
using ITC.Services.Email;
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
		private readonly IWalletRepository _walletRepository;
		private readonly IEmailService _emailService;
		private readonly IServiceProvider _serviceProvider;

		public AuthService(
			UserManager<ApplicationUser> userManager,
			IWalletRepository walletRepository,
			ITokenService tokenService,
			ILogger<AuthService> logger,
			IConfiguration configuration,
			IEmailService emailService,
			IMapper mapper,
			IServiceProvider serviceProvider)
		{
			_userManager = userManager;
			_tokenService = tokenService;
			_logger = logger;
			_walletRepository = walletRepository;
			_emailService = emailService;
			_serviceProvider = serviceProvider;

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
				UserName = registerDto.Email,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				EmailConfirmed = false,
				PhoneNumberConfirmed = true,
				FullName = registerDto.UserName,
				Address = registerDto.Address,
				Gender = registerDto.Gender ?? "Not Specified"
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

			// Tất cả Role đều được Approved ngay
			if (registerDto.Role.Equals("Customer"))
			{
				await _userManager.AddToRoleAsync(user, "Customer");
				user.ApprovalStatus = UserApprovalStatus.Approved;
			}
			else if (registerDto.Role.Equals("Talent"))
			{
				await _userManager.AddToRoleAsync(user, "Talent");
				user.ApprovalStatus = UserApprovalStatus.NoCertificate;
			}
			else
			{
				await _userManager.AddToRoleAsync(user, "Admin");
				user.ApprovalStatus = UserApprovalStatus.Approved;
			}
			await _userManager.UpdateAsync(user);

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var refreshToken = _tokenService.GenerateRefreshToken();
			await _emailService.SendConfirmationEmailAsync(user, token);
			// Save refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);
			await CreateWalletForUserAsync(user.Id);
			return new AuthResponseDto
			{
				Success = true,
				Message = "Registration successful. Please check your email to confirm your account."
			};
		}


		public async Task CreateWalletForUserAsync(Guid accountId)
		{
			var wallet = new Wallet
			{
				WalletId = Guid.NewGuid(),
				AccountId = accountId,
				Balance = 0
			};
			await _walletRepository.CreateWallet(wallet);
		}


		public async Task<AuthResponseDto> RegisterMBAsync(RegisterDto registerDto)
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
				UserName = registerDto.Email,
				Email = registerDto.Email,
				PhoneNumber = registerDto.PhoneNumber,
				EmailConfirmed = true,
				PhoneNumberConfirmed = true,
				FullName = registerDto.UserName,
				Address = registerDto.Address,
				Gender = registerDto.Gender ?? "Not Specified"
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

			// Tất cả Role đều được Approved ngay
			if (registerDto.Role.Equals("Customer"))
			{
				await _userManager.AddToRoleAsync(user, "Customer");
				user.ApprovalStatus = UserApprovalStatus.Approved;
			}
			else if (registerDto.Role.Equals("Talent"))
			{
				await _userManager.AddToRoleAsync(user, "Talent");
				user.ApprovalStatus = UserApprovalStatus.NoCertificate;
			}
			else
			{
				await _userManager.AddToRoleAsync(user, "Admin");
				user.ApprovalStatus = UserApprovalStatus.Approved;
			}
			await _userManager.UpdateAsync(user);

			var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

			var refreshToken = _tokenService.GenerateRefreshToken();
			// Save refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);
			await CreateWalletForUserAsync(user.Id);
			return new AuthResponseDto
			{
				Success = true,
				Message = "Registration successful. Please check your email to confirm your account."
			};
		}


		public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
		{

			var user = await _userManager.FindByEmailAsync(loginDto.UserName);

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

			// Lấy priority subscription
			int priority = 0;
			var subRepo = _serviceProvider.GetService(typeof(ITC.Repositories.Interface.IUserSubscriptionRepository)) as ITC.Repositories.Interface.IUserSubscriptionRepository;
			if (subRepo != null)
			{
				var activeSub = await subRepo.GetActiveSubscriptionAsync(user.Id);
				if (activeSub != null && activeSub.SubscriptionPlan != null)
				{
					switch (activeSub.SubscriptionPlan.Name.ToLower())
					{
						case "partnership": priority = 1; break;
						case "premium": priority = 2; break;
						case "advance": priority = 3; break;
					}
				}
			}

			return new AuthResponseDto
			{
				Success = true,
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				Message = "Login successful",
				User = userRes,
				Priority = priority
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

		//public async Task<UserResponse> LoginGoogle(GoogleLoginRequest request)
		//{
		//	var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token)
		//				  ?? throw new Exception("Invalid Google token.");

		//	string email = payload.Email;
		//	string name = payload.Name;
		//	string googleId = payload.Subject;

		//	var user = await _userManager.FindByEmailAsync(email);
		//	if (user == null)
		//	{
		//		user = new ApplicationUser
		//		{
		//			Email = email,
		//			UserName = googleId,
		//			FullName = payload.Name ?? "Unknown",
		//			Gender = "Not Specified",
		//			PhoneNumber = "Unknown",
		//			Address = "Not Provided",
		//			CreatedTime = DateTime.UtcNow,
		//			LastUpdatedTime = DateTime.UtcNow
		//		};

		//		var createResult = await _userManager.CreateAsync(user);
		//		if (!createResult.Succeeded)
		//		{
		//			var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
		//			throw new Exception($"User creation failed: {errors}");
		//		}
		//		var roleResult = await _userManager.AddToRoleAsync(user, "User");
		//		if (!roleResult.Succeeded)
		//		{
		//			var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
		//			_logger.LogError("Role assignment failed: {errors}", errors);
		//			throw new Exception($"Role assignment failed: {errors}");
		//		}
		//	}

		//	// Generate access token
		//	var token = await _tokenService.GenerateToken(user);

		//	// Generate refresh token
		//	var refreshToken = _tokenService.GenerateRefreshToken();

		//	// Hash the refresh token and store it in the database or override the existing refresh token
		//	using var sha256 = SHA256.Create();
		//	var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
		//	user.RefreshToken = Convert.ToBase64String(refreshTokenHash);
		//	user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2);


		//	var updateResult = await _userManager.UpdateAsync(user);
		//	if (!updateResult.Succeeded)
		//	{
		//		var errors = string.Join(", ", updateResult.Errors.Select(e => e.Description));
		//		_logger.LogError("Failed to update user: {errors}", errors);
		//		throw new Exception($"Failed to update user: {errors}");
		//	}

		//	var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);
		//	userResponse.AccessToken = token;
		//	userResponse.RefreshToken = refreshToken;
		//	userResponse.Address = user.Address;

		//	return userResponse;
		//}


		public async Task<UserResponse> LoginGoogle(GoogleLoginRequest request)
		{
			var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token)
						  ?? throw new Exception("Invalid Google token.");

			string email = payload.Email;
			string name = payload.Name;
			string googleId = payload.Subject;

			var user = await _userManager.FindByEmailAsync(email);
			if (user != null)
			{
				// Email đã tồn tại
				var roles = await _userManager.GetRolesAsync(user);
				var token = await _tokenService.GenerateToken(user);
				var refreshToken = _tokenService.GenerateRefreshToken();

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
			else
			{
				// Email chưa tồn tại, tạo mới user với Role = null
				var newUser = new ApplicationUser
				{
					UserName = googleId,
					Email = email,
					FullName = name,
					EmailConfirmed = true,
					RefreshToken = null,
					RefreshTokenExpiryTime = null,
				};

				var createResult = await _userManager.CreateAsync(newUser);
				if (!createResult.Succeeded)
				{
					var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
					_logger.LogError("Failed to create user: {errors}", errors);
					throw new Exception($"Failed to create user: {errors}");
				}

				await CreateWalletForUserAsync(newUser.Id);

				// Generate token + refresh token for newly created user
				var token = await _tokenService.GenerateToken(newUser);
				var refreshToken = _tokenService.GenerateRefreshToken();

				using var sha256 = SHA256.Create();
				var refreshTokenHash = sha256.ComputeHash(Encoding.UTF8.GetBytes(refreshToken));
				newUser.RefreshToken = Convert.ToBase64String(refreshTokenHash);
				newUser.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(2);
				await _userManager.UpdateAsync(newUser);

				var userResponse = _mapper.Map<ApplicationUser, UserResponse>(newUser);
				userResponse.AccessToken = token;
				userResponse.RefreshToken = refreshToken;
				userResponse.Address = newUser.Address;
				userResponse.Message = "Google account authenticated and user created.";

				return userResponse;
			}
		}



		public async Task<AuthResponseDto> AssignRoleToGoogleUserAsync(string email, string role)
		{
			if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(role))
			{
				return new AuthResponseDto
				{
					Success = false,
					Message = "Email and role are required."
				};
			}

			var user = await _userManager.FindByEmailAsync(email);
			if (user == null)
			{
				return new AuthResponseDto
				{
					Success = false,
					Message = "User not found. Please log in with Google first."
				};
			}

			// Gán role
			var roleResult = await _userManager.AddToRoleAsync(user, role);
			if (!roleResult.Succeeded)
			{
				var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
				_logger.LogError("Failed to assign role: {errors}", errors);
				return new AuthResponseDto
				{
					Success = false,
					Message = $"Failed to assign role: {errors}"
				};
			}

			// Tạo access token và refresh token
			var accessToken = await _tokenService.GenerateToken(user);
			var refreshToken = _tokenService.GenerateRefreshToken();

			// Lưu refresh token
			user.RefreshToken = refreshToken;
			user.RefreshTokenExpiryTime = DateTime.Now.AddDays(_refreshTokenExpiryDays);
			await _userManager.UpdateAsync(user);

			// Tạo ví cho user nếu chưa có (tuỳ trường hợp bạn có thể thêm check tồn tại)
			await CreateWalletForUserAsync(user.Id);

			var userResponse = _mapper.Map<ApplicationUser, UserResponse>(user);

			return new AuthResponseDto
			{
				Success = true,
				Message = "Role assigned and login successful.",
				AccessToken = accessToken,
				RefreshToken = refreshToken,
				User = userResponse
			};
		}


		/// <summary>
		/// Xác nhận email của người dùng dựa vào token và userId.
		/// </summary>
		public async Task<bool> ConfirmEmailAsync(string userId, string token)
		{
			var user = await _userManager.FindByIdAsync(userId);
			if (user == null) return false;

			var result = await _userManager.ConfirmEmailAsync(user, token);
			return result.Succeeded;
		}


		public async Task<bool> UpdateBankAccountAsync(Guid userId, UpdateBankAccountRequest request)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null) return false;

			user.BankAccountNumber = request.BankAccountNumber.Trim();
			user.BankName = request.BankName.Trim();
			user.BankAccountHolderName = request.BankAccountHolderName.Trim();
			user.LastUpdatedTime = CoreHelper.SystemTimeNow;

			var result = await _userManager.UpdateAsync(user);
			return result.Succeeded;
		}



	}
}

