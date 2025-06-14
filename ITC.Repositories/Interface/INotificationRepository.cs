using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface INotificationRepository
	{
		Task AddAsync(Notification notification);
		Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId);
		Task MarkAsReadAsync(Guid notificationId);
	}

}
