using System;
using ITC.Core.Enum;

namespace ITC.Services.DTOs.Complaint
{
    public class ComplaintCreateDto
    {
        public ComplaintType ComplaintType { get; set; }
        public Guid? RelatedJobApplicationId { get; set; }
        //public Guid? RelatedUserId { get; set; }
        public string Message { get; set; }
        public string Attachment { get; set; }
    }
} 