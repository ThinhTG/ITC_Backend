using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class JobCreateDto
	{
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public DateTime JobDate { get; set; }
		public Guid CustomerId { get; set; }
	}
}
