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
		public DateTimeOffset? ExpiredAt { get; set; }
		public string? PlanName { get; set; }
		public DateTimeOffset? SubscribedAt { get; set; }
		public int? RemainingPosts { get; set; }
		public int? RemainingApplications { get; set; }
		public TimeSpan? RemainingTime { get; set; }
	}

}
