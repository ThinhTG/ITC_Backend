using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class CreateJobRequest
	{
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
		public decimal SalaryAmount { get; set; }
		public IFormFile CompanyPdf { get; set; }
	}
}
