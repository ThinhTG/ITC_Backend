using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class CreateJobPostDto
	{
		public string JobTitle { get; set; }
		public string TranslationType { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
		public string? Description { get; set; }
		public string? UploadFileUrl { get; set; }

		public decimal? HourlyRate { get; set; }
		public decimal? PlatformServiceFee { get; set; }
		public decimal? TotalFee { get; set; }

		public string? CompanyName { get; set; }
		public string? CompanyDescription { get; set; }
		public string? CompanyLogoUrl { get; set; }

		public string? ContactEmail { get; set; }
		public string? ContactPhone { get; set; }
		public string? ContactAddress { get; set; }

		public string? WorkAddressLine { get; set; }
		public string? WorkCity { get; set; }
		public string? WorkPostalCode { get; set; }
		public string? WorkCountry { get; set; }

		public Guid CustomerId { get; set; }
	}
}
