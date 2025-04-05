using ITC.Core.Utils;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Mật khẩu người dùng
        /// </summary>
        public string? Password { get; set; }

        /// <summary>
        /// Ngày tạo tài khoản
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; }

        /// <summary>
        /// Lần cuối cập nhật tài khoản
        /// </summary>
        public DateTimeOffset LastUpdatedTime { get; set; }


        /// <summary>
        /// Ngày xóa tài khoản (nếu chưa xóa thì 
        /// </summary>
        public DateTimeOffset? DeletedTime { get; set; }

		//public ICollection<JobListing> JobListings { get; set; } = new List<JobListing>();
		//public ICollection<Application> Applications { get; set; } = new List<Application>();
		//public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
		//public ICollection<Review> Reviews { get; set; } = new List<Review>();

		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }

		// Thêm danh sách quan hệ với bảng UserRoles
		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

		public ApplicationUser()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
