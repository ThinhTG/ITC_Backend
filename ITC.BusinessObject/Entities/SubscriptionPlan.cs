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
		public bool IsBoosted { get; set; } // Dành cho gói đẩy top

		// Quyền lợi cho Customer
		public int? JobPostLimit { get; set; } // Null = không giới hạn
		public decimal ServiceFeePercentage { get; set; }

		// Quyền lợi cho Talent
		public int? ApplicationLimit { get; set; } // Null = không giới hạn
		public decimal CommissionFeePercentage { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ICollection<UserSubscription> UserSubscriptions { get; set; } = new List<UserSubscription>();
	}

}
