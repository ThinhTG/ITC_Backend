using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
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


		public async Task<SubscriptionStatusDto> GetUserSubscriptionStatusAsync(Guid userId)
		{
			var subscription = await _context.UserSubscriptions
				.Include(x => x.SubscriptionPlan)
				.Where(x => x.UserId == userId && x.IsActive && x.ExpiredAt > DateTime.UtcNow)
				.OrderByDescending(x => x.SubscribedAt)
				.FirstOrDefaultAsync();

			if (subscription == null)
			{
				return new SubscriptionStatusDto
				{
					IsActive = false
				};
			}

			return new SubscriptionStatusDto
			{
				IsActive = true,
				ExpiredAt = subscription.ExpiredAt,
				PlanName = subscription.SubscriptionPlan.Name
			};
		}
	}


}
