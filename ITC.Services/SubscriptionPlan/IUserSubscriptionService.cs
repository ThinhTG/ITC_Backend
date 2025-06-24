using ITC.Core.Base;
using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.SubscriptionPlan
{
	public interface IUserSubscriptionService
	{
		Task<BaseResponse<SubscriptionResponseDto>> SubscribeAsync(Guid userId, Guid planId);

		Task<SubscriptionResponseDto?> GetCurrentSubscriptionAsync(Guid userId);

		Task<SubscriptionStatusDto> CheckUserSubscriptionStatusAsync(Guid userId);

	}

}
