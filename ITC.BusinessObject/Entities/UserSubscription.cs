using ITC.BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class UserSubscription
	{
		public Guid Id { get; set; }
		public Guid UserId { get; set; }
		public Guid SubscriptionPlanId { get; set; }

		public DateTimeOffset SubscribedAt { get; set; } = DateTime.UtcNow;
		public DateTimeOffset ExpiredAt { get; set; }
		public bool IsActive { get; set; } = true;

		public ApplicationUser User { get; set; } = default!;
		public SubscriptionPlan SubscriptionPlan { get; set; } = default!;

		public bool IsCurrentlyValid => IsActive && ExpiredAt > DateTime.UtcNow;

	}

}
