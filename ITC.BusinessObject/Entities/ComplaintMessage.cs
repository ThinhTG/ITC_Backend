using System;

namespace ITC.BusinessObject.Entities
{
    public class ComplaintMessage
    {
        public Guid Id { get; set; }
        public Guid ComplaintId { get; set; }
        public Guid SenderId { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; } // file
        public DateTimeOffset SentAt { get; set; }
    }
} 