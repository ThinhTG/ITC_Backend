using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
     public  class UpdateBankAccountRequest
    {

		[Required]
		public string BankAccountNumber { get; set; } = default!;

		[Required]
		public string BankName { get; set; } = default!;

		[Required]
		public string BankAccountHolderName { get; set; } = default!;
	}
}
