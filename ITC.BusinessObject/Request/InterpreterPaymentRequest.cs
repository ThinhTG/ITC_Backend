using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class InterpreterPaymentRequest
	{
		public Guid JobId { get; set; }
		public Guid CustomerId { get; set; } // ID người thanh toán
		public decimal Amount { get; set; } // Số tiền cần trừ
	}

}
