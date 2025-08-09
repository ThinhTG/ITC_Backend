using ITC.BusinessObject.Entities;
using ITC.Core.Enum;
using ITC.Core.Utils;
using Microsoft.AspNetCore.Identity;
using static System.Net.Mime.MediaTypeNames;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public UserApprovalStatus ApprovalStatus { get; set; } = UserApprovalStatus.NoCertificate;
		public string? RejectReason { get; set; }
        /// <summary>
        /// Tên người dùng
        /// </summary>
        public string? FullName { get; set; }

        // Talent only
        public string? CertificateFiles { get; set; } // Dạng chuỗi JSON hoặc phân cách
        public string? Experience { get; set; }
        public string? PortraitUrl { get; set; }

		public string? AvatarUrl { get; set; }
		public string? Gender { get; set; }

		public string? Address { get; set; }

        /// <summary>
        /// Ngày tạo tài khoản
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Lần cuối cập nhật tài khoản
        /// </summary>
        public DateTimeOffset LastUpdatedTime { get; set; } = DateTimeOffset.UtcNow;


        /// <summary>
        /// Ngày xóa tài khoản (nếu chưa xóa thì 
        /// </summary>
        public DateTimeOffset? DeletedTime { get; set; } = DateTimeOffset.UtcNow;


		public int? orderCode { get; set; }



		public string? RefreshToken { get; set; }
		public DateTime? RefreshTokenExpiryTime { get; set; }

		// Thêm danh sách quan hệ với bảng UserRoles
		public virtual ICollection<ApplicationUserRole> UserRoles { get; set; } = new List<ApplicationUserRole>();

		public ApplicationUser()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
            ApprovalStatus = UserApprovalStatus.NoCertificate;
        }

		// Bank Account
		public string? BankAccountNumber { get; set; } // So tài khoản ngân hàng
		public string? BankName { get; set; } // Tên ngân hàng
		public string? BankAccountHolderName { get; set; } // Tên người nhận trong ngân hàng 

		// Free job posts tracking
		public int FreeJobPostsUsed { get; set; } = 0; // Số lần đã sử dụng post job free

		public virtual ICollection<TranslatorCertificate>? TranslatorCertificates { get; set; }

        public virtual ICollection<WithdrawalRequest> WithdrawalRequests { get; set; } = new List<WithdrawalRequest>();

	}
}
