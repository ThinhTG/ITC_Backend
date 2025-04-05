using ITC.BusinessObject.Identity;

namespace ITC.Services.TokenService
{
	public interface ITokenService
    {
        Task<string> GenerateToken(ApplicationUser user);

        string GenerateRefreshToken();
    }
}
