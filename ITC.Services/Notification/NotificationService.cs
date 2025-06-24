using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;
using Microsoft.AspNetCore.SignalR;
using ITC.Core.Hubs;

namespace ITC.Services.Notification
{
	public class NotificationService : INotificationService
	{
		private readonly INotificationRepository _notificationRepository;
		private readonly IHubContext<NotificationHub> _hubContext;

		public NotificationService(INotificationRepository notificationRepository, IHubContext<NotificationHub> hubContext)
		{
			_notificationRepository = notificationRepository;
			_hubContext = hubContext;
		}

		public async Task SendNotificationAsync(Guid receiverId, string title, string message)
		{
			var notification = new BusinessObject.Entities.Notifications
			{
				ReceiverUserId = receiverId,
				Title = title,
				Message = message
			};
			await _notificationRepository.InsertAsync(notification);
			await _notificationRepository.SaveAsync();

			// Gửi qua SignalR real-time
			await _hubContext.Clients.Group(receiverId.ToString())
				.SendAsync("ReceiveNotification", new { title, message });
		}

		public async Task<IEnumerable<Notifications>> GetNotificationsAsync(Guid userId)
		{
			return await _notificationRepository.GetByUserIdAsync(userId);
		}

		public async Task MarkAsReadAsync(Guid notificationId)
		{
			var notification = await _notificationRepository.GetByIdAsync(notificationId);
			if (notification != null)
			{
				notification.IsRead = true;
				_notificationRepository.Update(notification);
				await _notificationRepository.SaveAsync();
			}
		}
	}
}
