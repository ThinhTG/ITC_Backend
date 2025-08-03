using ITC.BusinessObject.DTOs.User;
using ITC.BusinessObject.Response;
using ITC.Services.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Lấy số lượng người dùng theo role Customer và Talent
        /// </summary>
        /// <returns></returns>
        [HttpGet("count-by-role")]
        //[Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserCountByRoleDto>> GetUserCountByRole()
        {
            var result = await _userService.GetUserCountByRoleAsync();
            return Ok(result);
        }

        [HttpGet("all-talents")]
        public async Task<ActionResult<UserResponse>> GetAllTalents()
        {
            var result = await _userService.GetAllTalentUsersAsync();
            return Ok(result);
        }

        [HttpGet("all-talents-with-certificates")]
        public async Task<ActionResult<TalentWithCertificatesResponse>> GetAllTalentsWithCertificates()
        {
            var result = await _userService.GetAllTalentUsersWithCertificatesAsync();
            return Ok(result);
        }

        [HttpGet("top-rated-talents")]
        public async Task<ActionResult<TalentWithCertificatesResponse>> GetTopRatedTalents([FromQuery] int count = 3)
        {
            var result = await _userService.GetTopRatedTalentsAsync(count);
            return Ok(result);
        }

	}
} 