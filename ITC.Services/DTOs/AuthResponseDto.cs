using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class AuthResponseDto
	{
		public bool Success { get; set; }
		public string Message { get; set; }
		public string AccessToken { get; set; }
		public string RefreshToken { get; set; }
	}
}
