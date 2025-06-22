using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
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
		private readonly IWalletRepository _walletRepo;
		private readonly IWalletTransactionRepository _walletTransactionRepo;

		public UserSubscriptionService(
			IUserSubscriptionRepository subscriptionRepo, 
			ISubscriptionPlanRepository planRepo,
			IWalletRepository walletRepo,
			IWalletTransactionRepository walletTransactionRepo)
		{
			_subscriptionRepo = subscriptionRepo;
			_planRepo = planRepo;
			_walletRepo = walletRepo;
			_walletTransactionRepo = walletTransactionRepo;
		}

		public async Task<SubscriptionResponseDto> SubscribeAsync(Guid userId, Guid planId)
		{
			var plan = await _planRepo.GetByIdAsync(planId);
			if (plan == null) throw new Exception("Gói không tồn tại");

			// Kiểm tra số dư ví
			var wallet = await _walletRepo.GetWalletByAccountIdAsync(userId);
			if (wallet == null) throw new Exception("Ví không tồn tại");
			if (wallet.Balance < plan.Price) throw new Exception("Số dư không đủ để đăng ký gói này");

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

			// Trừ tiền từ ví
			wallet.Balance -= plan.Price;
			await _walletRepo.UpdateWalletAsync(wallet);

			// Tạo transaction record
			var transaction = new WalletTransaction
			{
				WalletId = wallet.WalletId,
				Amount = -plan.Price, // Số âm vì là chi tiêu
				TransactionType = "Subscription",
				TransactionStatus = "Completed",
				TransactionBalance = wallet.Balance, // Số dư sau khi trừ
				Description = $"Đăng ký gói {plan.Name}",
				TransactionDate = DateTimeOffset.UtcNow
			};
			await _walletTransactionRepo.AddWalletTransactionAsync(transaction);

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


		public async Task<SubscriptionStatusDto> CheckUserSubscriptionStatusAsync(Guid userId)
		{
			return await _subscriptionRepo.GetUserSubscriptionStatusAsync(userId);
		}
	}

}
