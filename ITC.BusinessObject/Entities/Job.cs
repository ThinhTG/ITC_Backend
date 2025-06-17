using ITC.BusinessObject.Identity;
using ITC.Core.Enum;
using ITC.Core.Utils;
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
		public string TranslationType { get; set; } = string.Empty; // "Interpretation" / "Translation"
		public string SourceLanguage { get; set; } = string.Empty;
		public string TargetLanguage { get; set; } = string.Empty;
		public string? Description { get; set; }

		// Upload từ phía khách hàng (file cần dịch nếu là biên dịch)
		public string? UploadFileUrl { get; set; }

		//  THÔNG TIN DÀNH CHO PHIÊN DỊCH
		public DateTimeOffset? WorkingTime { get; set; } // Thời gian làm việc cụ thể
		public string? WorkAddressLine { get; set; }
		public string? WorkCity { get; set; }
		public string? WorkPostalCode { get; set; }
		public string? WorkCountry { get; set; }

		//  THÔNG TIN DÀNH CHO BIÊN DỊCH
		public DateTimeOffset? Deadline { get; set; } // Thời hạn hoàn thành
		public string? ResultFileUrl { get; set; } // File kết quả do BPDV upload
		public DateTimeOffset? CompletedAt { get; set; } // Ngày BPDV hoàn thành

		public int? CompletionOffsetMinutes { get; set; }  // độ trễ công việc khi hoàn thành ( so sánh thời gian hoàn thành với Deadline)   // <0: sớm, >0: trễ

		// Thanh toán tổng thể
		public decimal? HourlyRate { get; set; }
		public decimal? PlatformServiceFee { get; set; }
		public decimal TotalFee { get; set; }

		// Công ty/Tổ chức đăng tuyển
		public string? CompanyName { get; set; }
		public string? CompanyDescription { get; set; }
		public string? CompanyLogoUrl { get; set; }

		// Liên hệ
		public string? ContactEmail { get; set; }
		public string? ContactPhone { get; set; }
		public string? ContactAddress { get; set; }

		// Trạng thái tổng thể của job
		public int Status { get; set; } = (int)JobStatus.Open;
		public int RequiredHires { get; set; } = 1; // Default to 1 hire
		public int CurrentHires { get; set; } = 0; // Track current number of hires

		public DateTimeOffset CreatedAt { get; set; } = CoreHelper.SystemTimeNow;
		
		public ApplicationUser Customer { get; set; }
		public ICollection<JobApplication>? Applications { get; set; } = new List<JobApplication>();

		// Helper properties để tính toán trạng thái tổng thể
		public int TotalHiredInterpreters => Applications?.Count(a => a.ApplicationStatus == "1") ?? 0;
		public int TotalInProgressInterpreters => Applications?.Count(a => a.WorkStatus == (int)InterpreterWorkStatus.InProgress) ?? 0;
		public int TotalCompletedInterpreters => Applications?.Count(a => a.WorkStatus == (int)InterpreterWorkStatus.Completed) ?? 0;
		public bool IsFullyRecruited => TotalHiredInterpreters >= RequiredHires;
		public bool HasAnyInProgress => TotalInProgressInterpreters > 0;
		public bool IsAllCompleted => TotalCompletedInterpreters >= RequiredHires;
	}
}
