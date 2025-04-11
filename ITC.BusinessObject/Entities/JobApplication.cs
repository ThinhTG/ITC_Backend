 using ITC.BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class JobApplication
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid JobId { get; set; }
		public Job? Job { get; set; }

		public Guid InterpreterId { get; set; }
		public ApplicationUser? Interpreter { get; set; }

		public string Message { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;

		public int Status { get; set; } = 0; // 0: pending, 1: accepted, 2: rejected
	}

}
