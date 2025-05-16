using ITC.BusinessObject.Identity;
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
		public Guid CustomerId { get; set; } // Liên kết tới ApplicationUser (người đăng job)

		public string FullName { get; set; }
		public string Title { get; set; }
		public string WorkType { get; set; }
		public string TranslationLanguage { get; set; }
		public string ExperienceRequirement { get; set; }
		public string Education { get; set; }
		public string TranslationForm { get; set; }
		public string JobDescription { get; set; }
		public string RelevantCertificates { get; set; }

		public string ContactEmail { get; set; }
		public string ContactPhone { get; set; }
		public string WorkLocation { get; set; }

		public string SalaryType { get; set; }
		public decimal? SalaryAmount { get; set; }

		public string? CompanyPdfPath { get; set; }

		public int Status { get; set; } = 0; // Có thể định nghĩa enum sau
		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

		public ApplicationUser Customer { get; set; }
		public ICollection<JobApplication>? Applications { get; set; } = new List<JobApplication>();
	}
}
