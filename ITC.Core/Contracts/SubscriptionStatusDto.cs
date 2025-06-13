using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class SubscriptionStatusDto
	{
		public bool IsActive { get; set; }
		public DateTime? ExpiredAt { get; set; }
		public string? PlanName { get; set; }
	}

}
