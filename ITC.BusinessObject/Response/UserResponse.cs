using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Response
{
	public class UserResponse
	{
		public Guid Id { get; set; }
		public string FullName { get; set; }
		public string Email { get; set; }
		public string Gender { get; set; }
		public string? AvatarURL { get; set; }
		public string? PhoneNumber { get; set; } // Thêm số điện thoại
		public DateTime CreateAt { get; set; }
		public DateTime UpdateAt { get; set; }
		public string? AccessToken { get; set; }
		public string? RefreshToken { get; set; }
		public string Address { get; set; }


	}

}
