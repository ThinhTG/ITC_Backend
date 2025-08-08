using System;

namespace ITC.Services.DTOs.Job
{
    public class SubmittedFileDto
    {
        public Guid ApplicationId { get; set; }
        public Guid InterpreterId { get; set; }
        public string InterpreterName { get; set; } = string.Empty;
        public string InterpreterEmail { get; set; } = string.Empty;
        public string? IndividualResultFileUrl { get; set; }
        public DateTimeOffset? SubmittedAt { get; set; }
        public int WorkStatus { get; set; }
        public string WorkStatusText { get; set; } = string.Empty;
        public bool IsPaid { get; set; }
        public decimal? IndividualFee { get; set; }
    }
} 