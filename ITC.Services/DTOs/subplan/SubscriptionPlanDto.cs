using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.subplan
{
	public class SubscriptionPlanDto
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = string.Empty;
		public decimal Price { get; set; }
		public string Description { get; set; } = string.Empty;
		public int DurationInDays { get; set; }
		public bool IsBoosted { get; set; }
        
        // Quyền lợi cho Customer
        public int? JobPostLimit { get; set; } 
        public decimal ServiceFeePercentage { get; set; }

        // Quyền lợi cho Talent
        public int? ApplicationLimit { get; set; }
        public decimal CommissionFeePercentage { get; set; }
	}

}
