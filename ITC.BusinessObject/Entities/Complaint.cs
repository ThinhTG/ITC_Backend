using System;
using ITC.Core.Enum;

namespace ITC.BusinessObject.Entities
{
    public class Complaint
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
        public decimal AmountToPayTalent { get; set; } = 0;
        public decimal AmountToRefundCustomer { get; set; } = 0;
        public string ResolutionNotes { get; set; } = string.Empty;
        public DateTimeOffset? ResolvedAt { get; set; }
    }
} 