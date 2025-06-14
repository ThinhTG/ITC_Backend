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
	}
}
