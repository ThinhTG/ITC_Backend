using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.Payment
{
	public class PaymentResult
	{
		public bool IsSuccess { get; set; }
		public string? ErrorMessage { get; set; }

		public static PaymentResult Success() => new PaymentResult { IsSuccess = true };
		public static PaymentResult Fail(string error) => new PaymentResult { IsSuccess = false, ErrorMessage = error };
	}

}
