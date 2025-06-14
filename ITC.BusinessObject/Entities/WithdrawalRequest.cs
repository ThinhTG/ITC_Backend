using ITC.BusinessObject.Identity;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ITC.BusinessObject.Entities
{
    [Table("WithdrawalRequest")]
    public class WithdrawalRequest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WithdrawalRequestId { get; set; }

        [Required]
        public Guid AccountId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        public DateTime? ProcessedDate { get; set; }

        [Required]
        public string BankAccountNumber { get; set; }

        [Required]
        public string BankName { get; set; }

        [Required]
        public string BankAccountHolderName { get; set; }

        public string? Note { get; set; }

        public Guid? ProcessedBy { get; set; } // Staff who processed the request

        // Navigation properties
        public virtual ApplicationUser? Account { get; set; }
        public virtual ApplicationUser? ProcessedByUser { get; set; }
    }
} 