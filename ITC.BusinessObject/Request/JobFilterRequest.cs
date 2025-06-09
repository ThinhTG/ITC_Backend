using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class JobFilterRequest
	{
		public string? JobTitle { get; set; }
		public string? Location { get; set; }
		public List<string>? Categories { get; set; }
		public List<string>? SourceLanguages { get; set; }
		public List<string>? TargetLanguages { get; set; }
		public decimal? MinSalary { get; set; }
		public decimal? MaxSalary { get; set; }

		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}

}
