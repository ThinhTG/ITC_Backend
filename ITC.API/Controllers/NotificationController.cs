using ITC.BusinessObject.Entities;
using ITC.Services.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITC.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Lấy tất cả thông báo của một user, sắp xếp mới nhất trước.
        /// </summary>
        /// <param name="userId">ID của người dùng</param>
        /// <returns>Danh sách thông báo</returns>
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Notifications>>> GetNotifications(Guid userId)
        {
            var notifications = await _notificationService.GetNotificationsAsync(userId);
            return Ok(notifications);
        }

        /// <summary>
        /// Đánh dấu một thông báo là đã đọc.
        /// </summary>
        /// <param name="notificationId">ID của thông báo</param>
        /// <returns></returns>
        [HttpPost("{notificationId}/mark-as-read")]
        public async Task<IActionResult> MarkAsRead(Guid notificationId)
        {
            await _notificationService.MarkAsReadAsync(notificationId);
            return NoContent();
        }
    }
} 