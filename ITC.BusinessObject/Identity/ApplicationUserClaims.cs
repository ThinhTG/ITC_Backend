using Microsoft.AspNetCore.Identity;
using ITC.Core.Utils;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationUserClaims : IdentityUserClaim<Guid>
    {
        /// <summary>
        /// Ngày tạo
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; }

        /// <summary>
        /// Thời gian cập nhật cuối cùng
        /// </summary>

        public DateTimeOffset LastUpdatedTime { get; set; }

        /// <summary>
        /// Thời gian xóa
        /// </summary>
        public DateTimeOffset? DeletedTime { get; set; }

        public ApplicationUserClaims()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
