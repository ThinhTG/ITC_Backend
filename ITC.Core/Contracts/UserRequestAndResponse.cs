using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class UserRequestAndResponse
	{
		public class UpdateOrderCodeRequest
		{
			public int? orderCode { get; set; }
		}
	}
}
