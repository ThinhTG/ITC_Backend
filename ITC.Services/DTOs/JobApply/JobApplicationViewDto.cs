using ITC.BusinessObject.Identity;
using System;

namespace ITC.Services.DTOs.JobApply
{
	public class JobApplicationViewDto
	{
		public Guid ApplicationId { get; set; }
		public string JobTitle { get; set; } = default!;
		public string? Message { get; set; }
		public string? Status { get; set; }
		public DateTimeOffset CreatedAt { get; set; }
		public DateTimeOffset LastUpdatedAt { get; set; }
		public string? UploadFileUrl { get; set; }
		public string? Description { get; set; }
		public string TranslationType { get; set; }
		public string SourceLanguage { get; set; }
		public string TargetLanguage { get; set; }
		public DateTimeOffset? Deadline { get; set; }
		public decimal? HourlyRate { get; set; }
		public ApplicationUser? Interpreter { get; set; }
		public int? WorkStatus { get; set; }
		public int SubscriptionPriority { get; set; } // 0: No, 1: PartnerShip, 2: Premium, 3: Advance
	}
}
