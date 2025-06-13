using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.SubscriptionPlan
{
	public class UserSubscriptionService : IUserSubscriptionService
	{
		private readonly IUserSubscriptionRepository _subscriptionRepo;
		private readonly ISubscriptionPlanRepository _planRepo;

		public UserSubscriptionService(IUserSubscriptionRepository subscriptionRepo, ISubscriptionPlanRepository planRepo)
		{
			_subscriptionRepo = subscriptionRepo;
			_planRepo = planRepo;
		}

		public async Task<SubscriptionResponseDto> SubscribeAsync(Guid userId, Guid planId)
		{
			var plan = await _planRepo.GetByIdAsync(planId);
			if (plan == null) throw new Exception("Gói không tồn tại");

			var existing = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
			if (existing != null)
			{
				existing.IsActive = false;
			}

			var newSub = new UserSubscription
			{
				Id = Guid.NewGuid(),
				UserId = userId,
				SubscriptionPlanId = planId,
				SubscribedAt = DateTime.UtcNow,
				ExpiredAt = DateTime.UtcNow.AddDays(plan.DurationInDays),
				IsActive = true
			};

			await _subscriptionRepo.AddAsync(newSub);
			await _subscriptionRepo.SaveChangesAsync();

			return new SubscriptionResponseDto
			{
				PlanName = plan.Name,
				SubscribedAt = newSub.SubscribedAt,
				ExpiredAt = newSub.ExpiredAt,
				IsActive = newSub.IsActive
			};
		}

		public async Task<SubscriptionResponseDto?> GetCurrentSubscriptionAsync(Guid userId)
		{
			var sub = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
			if (sub == null) return null;

			return new SubscriptionResponseDto
			{
				PlanName = sub.SubscriptionPlan.Name,
				SubscribedAt = sub.SubscribedAt,
				ExpiredAt = sub.ExpiredAt,
				IsActive = sub.IsActive
			};
		}
	}

}
