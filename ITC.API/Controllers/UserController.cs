using ITC.BusinessObject.DTOs.User;
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
    }
} 