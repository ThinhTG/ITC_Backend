using ITC.Core.Enum;
using System;

namespace ITC.Services.DTOs.Withdrawal
{
    public class WithdrawalRequestDto
    {
        public Guid WithdrawalRequestId { get; set; }
        public Guid AccountId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Amount { get; set; }
        public WithdrawalStatus Status { get; set; }
        public DateTimeOffset RequestDate { get; set; }
        public DateTimeOffset? ProcessedDate { get; set; }
        public string BankAccountNumber { get; set; }
        public string BankName { get; set; }
        public string BankAccountHolderName { get; set; }
        public string? Note { get; set; }
        public Guid? ProcessedBy { get; set; }
        public string? ProcessedByName { get; set; }
    }

    public class CreateWithdrawalRequestDto
    {
        public decimal Amount { get; set; }
        public string? Note { get; set; }
    }

    public class UpdateWithdrawalRequestDto
    {
        public WithdrawalStatus Status { get; set; }
        public string? Note { get; set; }
    }
} 