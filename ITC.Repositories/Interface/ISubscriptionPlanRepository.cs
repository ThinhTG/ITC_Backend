using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface ISubscriptionPlanRepository
	{
		Task<SubscriptionPlan?> GetByIdAsync(Guid id);
		Task<List<SubscriptionPlan>> GetAllAsync();
	}

}
