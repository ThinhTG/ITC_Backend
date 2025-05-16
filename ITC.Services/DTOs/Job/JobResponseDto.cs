using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.Job
{
	public class JobResponseDto
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string Title { get; set; }
		public string WorkType { get; set; }
		public string TranslationLanguage { get; set; }
		public string JobDescription { get; set; }
		public string WorkLocation { get; set; }
		public string SalaryType { get; set; }
		public decimal? SalaryAmount { get; set; }
		public DateTime CreatedAt { get; set; }
	}

}
