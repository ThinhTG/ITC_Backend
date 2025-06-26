using System;
using ITC.Core.Enum;

namespace ITC.Services.DTOs.Complaint
{
    public class ComplaintDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid? AdminId { get; set; }
        public ComplaintType ComplaintType { get; set; }
        public Guid? RelatedJobApplicationId { get; set; }
        public Guid? RelatedUserId { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
        public decimal AmountToPayTalent { get; set; }
        public decimal AmountToRefundCustomer { get; set; }
        public string ResolutionNotes { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
    }
} 