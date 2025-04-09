using ITC.BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class Job
	{
		public Guid Id { get; set; } = Guid.NewGuid();

		public string Title { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		public string Location { get; set; } = string.Empty;

		public DateTime JobDate { get; set; }

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		/// <summary>
		/// 0 = Pending, 1 = Approved, 2 = Rejected
		/// </summary>
		public int Status { get; set; } = 0;

		public Guid CustomerId { get; set; }

		public ApplicationUser? Customer { get; set; }

		public ICollection<JobApplication>? Applications { get; set; }
	}
}
