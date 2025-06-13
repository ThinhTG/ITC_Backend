using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class TranslatorCertificateCreateUpdateDto
	{
		public string? Title { get; set; }
		public int? Experience { get; set; }
		public string? Education { get; set; }
		public string? Website { get; set; }
		public string? CvFileUrl { get; set; }
		public string? PhotoUrl { get; set; }
		public string? WorkType { get; set; }
		public string? TranslationForm { get; set; }
		public string? TranslationLanguage { get; set; }
		public string? CertificateNames { get; set; }
		public string? CertificateFileUrl { get; set; }
	}

}
