using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Notification
{
	public interface INotificationService
	{
		Task SendNotificationAsync(Guid receiverId, string title, string message);
		Task<IEnumerable<Notifications>> GetNotificationsAsync(Guid userId);
		Task MarkAsReadAsync(Guid notificationId);
	}
}
