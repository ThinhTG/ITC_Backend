using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ITC.Services.Complaint;
using ITC.Services.DTOs.Complaint;
using System.Security.Claims;
using ITC.Core.Base;

namespace ITC.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComplaintController : ControllerBase
    {
        private readonly IComplaintService _complaintService;
        public ComplaintController(IComplaintService complaintService)
        {
            _complaintService = complaintService;
        }

        /// <summary>
        /// Tạo khiếu nại mới (kèm tin nhắn đầu tiên và đính kèm dạng string).
        /// </summary>
        /// <param name="dto">Thông tin khiếu nại và tin nhắn đầu tiên</param>
        /// <returns>Thông tin khiếu nại vừa tạo</returns>
        // POST: api/complaint
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateComplaint([FromBody] ComplaintCreateDto dto)
        {
            // Lấy userId của người khiếu nại
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
			var result = await _complaintService.CreateComplaintAsync(userId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách khiếu nại của người dùng hiện tại.
        /// </summary>
        /// <returns>Danh sách khiếu nại</returns>
        // GET: api/complaint
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetMyComplaints()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _complaintService.GetComplaintsByUserAsync(userId);
            return Ok(result);
        }

        /// <summary>
        /// Lấy tất cả khiếu nại (chỉ cho admin).
        /// </summary>
        /// <returns>Danh sách tất cả khiếu nại</returns>
        // GET: api/complaint/all (admin)
        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllComplaints()
        {
            var result = await _complaintService.GetAllComplaintsAsync();
            return Ok(result);
        }

        /// <summary>
        /// Lấy danh sách tin nhắn của một khiếu nại.
        /// </summary>
        /// <param name="id">Id của khiếu nại</param>
        /// <returns>Danh sách tin nhắn</returns>
        // GET: api/complaint/{id}/messages
        [HttpGet("{id}/messages")]
        [Authorize]
        public async Task<IActionResult> GetMessages(Guid id)
        {
            var result = await _complaintService.GetMessagesAsync(id);
            return Ok(result);
        }

        /// <summary>
        /// Gửi tin nhắn mới vào khiếu nại (có thể kèm đính kèm dạng string).
        /// </summary>
        /// <param name="id">Id của khiếu nại</param>
        /// <param name="dto">Nội dung tin nhắn</param>
        /// <returns>Tin nhắn vừa gửi</returns>
        // POST: api/complaint/{id}/messages
        [HttpPost("{id}/messages")]
        [Authorize]
        public async Task<IActionResult> SendMessage(Guid id, [FromBody] ComplaintMessageCreateDto dto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var result = await _complaintService.SendMessageAsync(id, userId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Đổi trạng thái khiếu nại (chỉ cho admin).
        /// </summary>
        /// <param name="id">Id của khiếu nại</param>
        /// <param name="status">Trạng thái mới (Processing, Responded, Closed)</param>
        /// <returns></returns>
        // PATCH: api/complaint/{id}/status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] int status)
        {
            await _complaintService.ChangeStatusAsync(id, status);
            return NoContent();
        }

        /// <summary>
        /// Admin xử lý khiếu nại tài chính (refund/thanh toán/ghi chú).
        /// </summary>
        /// <param name="id">Id của khiếu nại</param>
        /// <param name="dto">Thông tin xử lý khiếu nại</param>
        /// <returns></returns>
        [HttpPost("{id}/resolve")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ResolveComplaint(Guid id, [FromBody] ComplaintResolutionDto dto)
        {
            await _complaintService.ResolveComplaintAsync(id, dto);
            return Ok("The financial complaint has been successfully processed.");
        }
    }
} 