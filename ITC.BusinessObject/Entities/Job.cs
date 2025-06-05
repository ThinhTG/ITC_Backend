using ITC.BusinessObject.Identity;
using ITC.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class Job
	{
		public Guid Id { get; set; }
		public Guid CustomerId { get; set; }

		// Basic Info
		public string JobTitle { get; set; } = string.Empty;
		public string TranslationType { get; set; } = string.Empty;
		public string SourceLanguage { get; set; } = string.Empty;
		public string TargetLanguage { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? UploadFileUrl { get; set; }


		// Payment Info
		public decimal? HourlyRate { get; set; }
		public decimal? PlatformServiceFee { get; set; }
		public decimal? TotalFee { get; set; }

		// Company/Org Info
		public string? CompanyName { get; set; }
		public string? CompanyDescription { get; set; }
		public string? CompanyLogoUrl { get; set; }

		// Contact Info
		public string? ContactEmail { get; set; }
		public string? ContactPhone { get; set; }
		public string? ContactAddress { get; set; }

		// Work Location
		public string? WorkAddressLine { get; set; }
		public string? WorkCity { get; set; }
		public string? WorkPostalCode { get; set; }
		public string? WorkCountry { get; set; }

		public int Status { get; set; } = (int)JobStatus.Open;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ApplicationUser Customer { get; set; }
		public ICollection<JobApplication>? Applications { get; set; } = new List<JobApplication>();
	}
}
