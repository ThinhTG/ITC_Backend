using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class SubscriptionRequestDto
	{
		public Guid SubscriptionPlanId { get; set; }
	}
	public class SubscriptionResponseDto
	{
		public string PlanName { get; set; }
		public DateTime SubscribedAt { get; set; }
		public DateTime ExpiredAt { get; set; }
		public bool IsActive { get; set; }
	}

}
