using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ITC.Core.Constants.EndPointAPI;
using System.Text.Json.Serialization;

namespace ITC.BusinessObject.Entities
{
	[Table("WalletTransaction")]
	public class WalletTransaction
	{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public Guid WalletTransactionId { get; set; }

		[Required]
		public Guid WalletId { get; set; }
		[Required]
		public decimal Amount { get; set; }
		[Required]
		public string TransactionType { get; set; } = string.Empty;
		[Required]
		public string TransactionStatus { get; set; } = string.Empty;
		[Required]
		public DateTimeOffset TransactionDate { get; set; }
		[Required]
		public decimal TransactionBalance { get; set; }

		public string? Description { get; set; } = string.Empty;
		public int? OrderId { get; set; }
		public DateTimeOffset CreateAt { get; set; }

		[JsonIgnore]
		public virtual Wallet? Wallet { get; set; }
		//public virtual Order? Order { get; set; }
	}
}
