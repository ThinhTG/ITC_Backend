 using ITC.BusinessObject.Identity;
using ITC.Core.Utils;
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
		public DateTimeOffset CreatedAt { get; set; } = CoreHelper.SystemTimeNow;
		public DateTimeOffset LastUpdatedAt { get; set; } = CoreHelper.SystemTimeNow;
		public string Status { get; set; } // 0: pending, 1: accepted, 2: rejected 3: Done
	}

}
