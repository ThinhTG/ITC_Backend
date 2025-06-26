using System;

namespace ITC.Services.DTOs.Complaint
{
    public class ComplaintMessageDto
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
        public DateTimeOffset SentAt { get; set; }
    }
} 