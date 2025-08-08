using System;
using System.Collections.Generic;

namespace ITC.Services.DTOs.Job
{
    public class JobDetailsDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        
        // Basic Info
        public string JobTitle { get; set; } = string.Empty;
        public string TranslationType { get; set; } = string.Empty;
        public string SourceLanguage { get; set; } = string.Empty;
        public string TargetLanguage { get; set; } = string.Empty;
        public string? Description { get; set; }
        
        // Upload file
        public string? UploadFileUrl { get; set; }
        
        // Working info for interpretation
        public DateTimeOffset? WorkingTime { get; set; }
        public string? WorkAddressLine { get; set; }
        public string? WorkCity { get; set; }
        public string? WorkPostalCode { get; set; }
        public string? WorkCountry { get; set; }
        
        // Deadline info for translation
        public DateTimeOffset? Deadline { get; set; }
        public string? ResultFileUrl { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public int? CompletionOffsetMinutes { get; set; }
        
        // Payment info
        public decimal? HourlyRate { get; set; }
        public decimal? PlatformServiceFee { get; set; }
        public decimal TotalFee { get; set; }
        
        // Company info
        public string? CompanyName { get; set; }
        public string? CompanyDescription { get; set; }
        public string? CompanyLogoUrl { get; set; }
        
        // Contact info
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactAddress { get; set; }
        
        // Status info
        public int Status { get; set; }
        public int RequiredHires { get; set; }
        public int CurrentHires { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        
        // Customer info (simplified)
        public string? CustomerName { get; set; }
        public string? CustomerEmail { get; set; }
        
        // Applications info (simplified)
        public List<JobApplicationSummaryDto> Applications { get; set; } = new List<JobApplicationSummaryDto>();
        
        // Submitted files info (for customer to easily view submitted work)
        public List<SubmittedFileDto> SubmittedFiles { get; set; } = new List<SubmittedFileDto>();
        
        // Helper properties
        public int TotalHiredInterpreters { get; set; }
        public int TotalInProgressInterpreters { get; set; }
        public int TotalCompletedInterpreters { get; set; }
        public bool IsFullyRecruited { get; set; }
        public bool HasAnyInProgress { get; set; }
        public bool IsAllCompleted { get; set; }
    }
    
    public class JobApplicationSummaryDto
    {
        public Guid Id { get; set; }
        public Guid InterpreterId { get; set; }
        public string InterpreterName { get; set; } = string.Empty;
        public string InterpreterEmail { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset LastUpdatedAt { get; set; }
        public string ApplicationStatus { get; set; } = string.Empty;
        public int WorkStatus { get; set; }
        public bool IsPaid { get; set; }
        public decimal? IndividualFee { get; set; }
        public DateTimeOffset? PaidAt { get; set; }
        public string? IndividualResultFileUrl { get; set; }
        public DateTimeOffset? StartedAt { get; set; }
        public DateTimeOffset? CompletedAt { get; set; }
        public int? CompletionOffsetMinutes { get; set; }
    }
} 