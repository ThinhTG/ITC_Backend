using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface IUserSubscriptionRepository
	{
		Task<UserSubscription?> GetActiveSubscriptionAsync(Guid userId);
		Task AddAsync(UserSubscription subscription);
		Task SaveChangesAsync();

		Task<SubscriptionStatusDto> GetUserSubscriptionStatusAsync(Guid userId);

		Task<IEnumerable<UserSubscription>> GetAllAsync();
		Task<List<UserSubscription>> GetActiveSubscriptionsForUsersAsync(IEnumerable<Guid> userIds);
	}


}
