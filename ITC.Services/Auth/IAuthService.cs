using ITC.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Auth
{
	public interface IAuthService
	{
		Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
		Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
		Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenDto refreshTokenDto);
		Task<bool> LogoutAsync(string userName);
	}
}
