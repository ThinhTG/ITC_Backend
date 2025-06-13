using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class SubscriptionPlan
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = default!;
		public decimal Price { get; set; }
		public string? Description { get; set; }
		public int DurationInDays { get; set; } // số ngày sử dụng
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
	}

}
