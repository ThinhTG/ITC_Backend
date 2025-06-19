using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class JobApplicationCardDto
	{
		public Guid ApplicationId { get; set; }
		public Guid JobId { get; set; }
		public string JobTitle { get; set; } = string.Empty;
		public string Price { get; set; } = string.Empty;
		public string Status { get; set; } = string.Empty; // "Pending", "Accepted", "Rejected"
		public DateTimeOffset? DeadLine { get; set; } 
		public DateTimeOffset CreatedDate { get; set; }
		// Thêm thông tin file upload của customer để talent có thể tải về
		public string? UploadFileUrl { get; set; }
		public string? Description { get; set; }
		public string TranslationType { get; set; } = string.Empty;
		public string SourceLanguage { get; set; } = string.Empty;
		public string TargetLanguage { get; set; } = string.Empty;

		public int WorkStatus { get; set; }  
	}
}
