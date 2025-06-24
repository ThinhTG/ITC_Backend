using ITC.BusinessObject.Entities;
using ITC.Core.Base;
using ITC.Core.Constants;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
using ITC.Services.Privilege;
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
		private readonly IPrivilegeService _privilegeService;

		public UserSubscriptionService(
			IUserSubscriptionRepository subscriptionRepo, 
			ISubscriptionPlanRepository planRepo,
			IWalletRepository walletRepo,
			IWalletTransactionRepository walletTransactionRepo,
			IPrivilegeService privilegeService)
		{
			_subscriptionRepo = subscriptionRepo;
			_planRepo = planRepo;
			_walletRepo = walletRepo;
			_walletTransactionRepo = walletTransactionRepo;
			_privilegeService = privilegeService;
		}

		//public async Task<SubscriptionResponseDto> SubscribeAsync(Guid userId, Guid planId)
		//{
		//	var plan = await _planRepo.GetByIdAsync(planId);
		//	if (plan == null) throw new Exception("Gói không tồn tại");

		//	// Kiểm tra số dư ví
		//	var wallet = await _walletRepo.GetWalletByAccountIdAsync(userId);
		//	if (wallet == null) throw new Exception("Ví không tồn tại");
		//	if (wallet.Balance < plan.Price) throw new Exception("Số dư không đủ để đăng ký gói này");

		//	var existing = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
		//	if (existing != null)
		//	{
		//		existing.IsActive = false;
		//	}

		//	var newSub = new UserSubscription
		//	{
		//		Id = Guid.NewGuid(),
		//		UserId = userId,
		//		SubscriptionPlanId = planId,
		//		SubscribedAt = DateTime.UtcNow,
		//		ExpiredAt = DateTime.UtcNow.AddDays(plan.DurationInDays),
		//		IsActive = true
		//	};

		//	// Trừ tiền từ ví
		//	wallet.Balance -= plan.Price;
		//	await _walletRepo.UpdateWalletAsync(wallet);

		//	// Tạo transaction record
		//	var transaction = new WalletTransaction
		//	{
		//		WalletId = wallet.WalletId,
		//		Amount = -plan.Price, // Số âm vì là chi tiêu
		//		TransactionType = "Subscription",
		//		TransactionStatus = "Completed",
		//		TransactionBalance = wallet.Balance, // Số dư sau khi trừ
		//		Description = $"Đăng ký gói {plan.Name}",
		//		TransactionDate = DateTimeOffset.UtcNow
		//	};
		//	await _walletTransactionRepo.AddWalletTransactionAsync(transaction);

		//	await _subscriptionRepo.AddAsync(newSub);
		//	await _subscriptionRepo.SaveChangesAsync();

		//	return new SubscriptionResponseDto
		//	{
		//		PlanName = plan.Name,
		//		SubscribedAt = newSub.SubscribedAt,
		//		ExpiredAt = newSub.ExpiredAt,
		//		IsActive = newSub.IsActive
		//	};
		//}


		public async Task<BaseResponse<SubscriptionResponseDto>> SubscribeAsync(Guid userId, Guid planId)
		{
			var plan = await _planRepo.GetByIdAsync(planId);
			if (plan == null)
			{
				return new BaseResponse<SubscriptionResponseDto>(
					StatusCodeHelper.BadRequest,
					ResponseCodeConstants.NOT_FOUND,
					"Gói không tồn tại");
			}

			var wallet = await _walletRepo.GetWalletByAccountIdAsync(userId);
			if (wallet == null)
			{
				return new BaseResponse<SubscriptionResponseDto>(
					StatusCodeHelper.BadRequest,
					ResponseCodeConstants.NOT_FOUND,
					"Ví không tồn tại");
			}

			if (wallet.Balance < plan.Price)
			{
				return new BaseResponse<SubscriptionResponseDto>(
					StatusCodeHelper.BadRequest,
					ResponseCodeConstants.BALANCE_NOT_ENOUGH,
					"Số dư không đủ để đăng ký gói này");
			}

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

			wallet.Balance -= plan.Price;
			await _walletRepo.UpdateWalletAsync(wallet);

			var transaction = new WalletTransaction
			{
				WalletId = wallet.WalletId,
				Amount = -plan.Price,
				TransactionType = "Subscription",
				TransactionStatus = "Completed",
				TransactionBalance = wallet.Balance,
				Description = $"Đăng ký gói {plan.Name}",
				TransactionDate = DateTimeOffset.UtcNow
			};
			await _walletTransactionRepo.AddWalletTransactionAsync(transaction);

			await _subscriptionRepo.AddAsync(newSub);
			await _subscriptionRepo.SaveChangesAsync();

			var responseDto = new SubscriptionResponseDto
			{
				PlanName = plan.Name,
				SubscribedAt = newSub.SubscribedAt,
				ExpiredAt = newSub.ExpiredAt,
				IsActive = newSub.IsActive
			};

			return BaseResponse<SubscriptionResponseDto>.OkResponse(responseDto);
		}

		public async Task<SubscriptionStatusDto?> GetCurrentSubscriptionAsync(Guid userId)
		{
			var sub = await _subscriptionRepo.GetActiveSubscriptionAsync(userId);
			if (sub == null || sub.SubscriptionPlan == null)
			{
				return new SubscriptionStatusDto
				{
					IsActive = false,
					PlanName = "No active subscription"
				};
			}

			var remainingPosts = await _privilegeService.GetRemainingJobPostsAsync(userId);
			var remainingApplications = await _privilegeService.GetRemainingApplicationsAsync(userId);
			var remainingTime = sub.ExpiredAt - DateTimeOffset.UtcNow;
			if (remainingTime < TimeSpan.Zero) remainingTime = TimeSpan.Zero;

			return new SubscriptionStatusDto
			{
				PlanName = sub.SubscriptionPlan.Name,
				SubscribedAt = sub.SubscribedAt,
				ExpiredAt = sub.ExpiredAt,
				IsActive = sub.IsActive,
				RemainingPosts = remainingPosts == int.MaxValue ? null : remainingPosts,
				RemainingApplications = remainingApplications == int.MaxValue ? null : remainingApplications,
				RemainingTime = remainingTime
			};
		}


		public async Task<SubscriptionStatusDto> CheckUserSubscriptionStatusAsync(Guid userId)
		{
			return await _subscriptionRepo.GetUserSubscriptionStatusAsync(userId);
		}
	}

}
