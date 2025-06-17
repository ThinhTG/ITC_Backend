using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.JobApply
{
	public class JobApplicationViewDto
	{
		public Guid ApplicationId { get; set; }
		public string JobTitle { get; set; } = string.Empty;
		public string Message { get; set; } = string.Empty;
		public string Status { get; set; } = "0"; // 0: pending, 1: accepted, 2: rejected
		public DateTime CreatedAt { get; set; }
		public DateTime LastUpdatedAt { get; set; }
		// Thêm thông tin file upload của customer để talent có thể tải về
		public string? UploadFileUrl { get; set; }
		public string? Description { get; set; }
		public string TranslationType { get; set; } = string.Empty;
		public string SourceLanguage { get; set; } = string.Empty;
		public string TargetLanguage { get; set; } = string.Empty;
		public DateTimeOffset? Deadline { get; set; }
		public decimal? HourlyRate { get; set; }
	}
}
