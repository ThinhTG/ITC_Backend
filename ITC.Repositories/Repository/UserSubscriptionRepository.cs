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
	public class UserSubscriptionRepository : IUserSubscriptionRepository
	{
		private readonly ITCDbContext _context;

		public UserSubscriptionRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId)
		{
			return await _context.UserSubscriptions
				.Include(us => us.SubscriptionPlan)
				.FirstOrDefaultAsync(us => us.UserId == userId && us.IsActive && us.ExpiredAt > DateTime.UtcNow);
		}

		public async Task AddAsync(UserSubscription subscription)
		{
			await _context.UserSubscriptions.AddAsync(subscription);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}


}
