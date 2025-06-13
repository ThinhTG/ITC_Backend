using ITC.BusinessObject.Entities;
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
	}


}
