using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ITC.Core.Constants.EndPointAPI;

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
		public string TransactionDate { get; set; } = string.Empty;
		[Required]
		public string TransactionBalance { get; set; } = string.Empty;
		public int? OrderId { get; set; }

		public virtual Wallet? Wallet { get; set; }
		//public virtual Order? Order { get; set; }
	}
}
