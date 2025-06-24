using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface INotificationRepository : IGenericRepository<Notifications>
	{
		Task<IEnumerable<Notifications>> GetUnreadByUserIdAsync(Guid userId);
		Task MarkAsReadAsync(Guid notificationId);
		Task<IEnumerable<Notifications>> GetByUserIdAsync(Guid userId);
	}

}
