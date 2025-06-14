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
			var notification = new BusinessObject.Entities.Notification
			{
				ReceiverUserId = receiverId,
				Title = title,
				Message = message
			};
			await _notificationRepository.AddAsync(notification);

			// Gửi qua SignalR real-time
			await _hubContext.Clients.Group(receiverId.ToString())
				.SendAsync("ReceiveNotification", new { title, message });
		}
	}

}
