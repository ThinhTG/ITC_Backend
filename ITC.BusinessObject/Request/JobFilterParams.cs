using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class JobFilterParams
	{
		public string? Search { get; set; }      // ô “Job title”
		public string? Location { get; set; }      // combo “Choose Location”

		public List<string>? Categories { get; set; }      // Translation / Interpretation
		public List<string>? Expertises { get; set; }      // Commerce, Media & Broadcast …

		public List<string>? SourceLanguages { get; set; } // Japanese, English…
		public List<string>? TargetLanguages { get; set; }

		public decimal? MinSalary { get; set; }      // slider “Salary: $0 – $9999”
		public decimal? MaxSalary { get; set; }

		public int PageIndex { get; set; } = 1;
		public int PageSize { get; set; } = 10;
	}

}
