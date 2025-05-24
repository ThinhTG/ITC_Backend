using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class JobDTO
	{
		public Guid Id { get; set; }
		public string JobTitle { get; set; } = string.Empty;
		public string TranslationType { get; set; } = string.Empty;
		public string SourceLanguage { get; set; } = string.Empty;
		public string TargetLanguage { get; set; } = string.Empty;
		public string? CompanyName { get; set; }
		public decimal? TotalFee { get; set; }
		public DateTime CreatedAt { get; set; }
	}
}
