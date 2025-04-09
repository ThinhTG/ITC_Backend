using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class JobDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public string Location { get; set; } = string.Empty;
		public DateTime JobDate { get; set; }
		public int Status { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
