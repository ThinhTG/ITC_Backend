using ITC.Services.DTOs.subplan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.SubscriptionPlan
{
	public interface ISubscriptionPlanService
	{
		Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync();
		Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid id);
	}

}
