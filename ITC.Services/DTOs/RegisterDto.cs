using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs
{
	public class RegisterDto
	{
		[Required]
		[StringLength(50)]
		public string UserName { get; set; }

		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[StringLength(100, MinimumLength = 6)]
		public string Password { get; set; }

		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string ConfirmPassword { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Role { get; set; }
	}
}
