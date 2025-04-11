using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class JobFilterDto
	{
		public string? Location { get; set; }
		public int? Status { get; set; }
		public DateTime? FromDate { get; set; }
		public DateTime? ToDate { get; set; }

		public int Page { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}
}
