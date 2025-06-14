using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Repository
{
	public class NotificationRepository : INotificationRepository
	{
		private readonly ITCDbContext _context;

		public NotificationRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Notification notification)
		{
			await _context.Notifications.AddAsync(notification);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(Guid userId)
		{
			return await _context.Notifications
				.Where(n => n.ReceiverUserId == userId && !n.IsRead)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();
		}

		public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(Guid userId)
		{
			return await _context.Notifications
				.Where(n => n.ReceiverUserId == userId)
				.OrderByDescending(n => n.CreatedAt)
				.ToListAsync();
		}

		public async Task MarkAsReadAsync(Guid notificationId)
		{
			var notification = await _context.Notifications.FindAsync(notificationId);
			if (notification != null)
			{
				notification.IsRead = true;
				await _context.SaveChangesAsync();
			}
		}
	}

}
