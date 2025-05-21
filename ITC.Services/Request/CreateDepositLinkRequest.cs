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
		public string returnUrl = "https://mystic-blind-box.web.app/wallet-success";
		public string cancelUrl = "https://mystic-blind-box.web.app/wallet-fail";
	}
}
