using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Request
{
	public class UserUpdateDto
	{
		[Required]
		public string Id { get; set; }

		public string? FullName { get; set; }

		[RegularExpression(@"^0[0-9]{9}$", ErrorMessage = "Số điện thoại phải bắt đầu bằng 0 và có 10 chữ số")]
		public string? PhoneNumber { get; set; }

		public string? AvatarUrl { get; set; }

		public string? Gender { get; set; }

		public string? Address { get; set; }
	}
}
