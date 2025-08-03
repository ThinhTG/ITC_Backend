using ITC.Core.Contracts;
using System;
using System.Collections.Generic;

namespace ITC.BusinessObject.Response
{
    public class TalentWithCertificatesResponse
    {
        public Guid Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string? AvatarURL { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public string Address { get; set; }
        
        // Talent specific fields
        public string? CertificateFiles { get; set; }
        public string? Experience { get; set; }
        public string? PortraitUrl { get; set; }
        public string? ApprovalStatus { get; set; }
        public string? RejectReason { get; set; }
        public bool IsBoosted { get; set; }
        public int Priority { get; set; }
        
        // Bank Account
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? BankAccountHolderName { get; set; }
        
        // Certificates
        public List<TranslatorCertificateDto> Certificates { get; set; } = new List<TranslatorCertificateDto>();
        
        // Rating Information
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public int[] StarCounts { get; set; } = new int[5]; // 0: 1 sao, 4: 5 sao
    }
} 