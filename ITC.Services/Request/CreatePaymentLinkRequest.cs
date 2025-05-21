using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Request
{
	public class CreatePaymentLinkRequest
	{
		public int orderId { get; set; }
		public string description = "Payment ";
		public int price { get; set; }
		public string returnUrl = "https://ITC.web.app/payment-success";
		public string cancelUrl = "https://ITC.web.app/payment-fail";
	}
}
