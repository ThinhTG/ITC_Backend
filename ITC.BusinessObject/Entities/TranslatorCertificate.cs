using ITC.BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace ITC.BusinessObject.Entities
{
	public class TranslatorCertificate
	{
		[Key]
		public Guid Id { get; set; }

		public Guid ApplicationUserId { get; set; }

		[ForeignKey(nameof(ApplicationUserId))]
		[JsonIgnore]
		public virtual ApplicationUser? User { get; set; }

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

		public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
		public DateTimeOffset? UpdatedAt { get; set; } 
	}
}
