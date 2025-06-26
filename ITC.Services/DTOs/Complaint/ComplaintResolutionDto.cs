using System;
using ITC.Core.Enum;

namespace ITC.Services.DTOs.Complaint
{
    public class ComplaintResolutionDto
    {
        public decimal AmountToPayTalent { get; set; }
        public decimal AmountToRefundCustomer { get; set; }
        public string ResolutionNotes { get; set; }
        public ComplaintStatus Status { get; set; }
        public DateTimeOffset? ResolvedAt { get; set; }
    }
} 