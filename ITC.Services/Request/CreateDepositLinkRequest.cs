using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Request
{
	public class CreateDepositLinkRequest
	{
		public string? accountId { get; set; }
		public string description = "Deposit ";
		public int price { get; set; }
		public string returnUrl = "http://localhost:3000/deposit_success";
		public string cancelUrl = "http://localhost:3000/deposit_fail";
	}
}
