using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class JobApplicationCardDto
	{
		public Guid ApplicationId { get; set; }
		public string JobTitle { get; set; } = string.Empty;
		public string Price { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty; // "Pending", "Accepted", "Rejected"
		public DateTimeOffset CreatedDate { get; set; }
	}
}
