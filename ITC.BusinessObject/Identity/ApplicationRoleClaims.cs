using ITC.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationRoleClaims : IdentityRoleClaim<Guid>

    {
        /// <summary>
        /// Thời gian tạo
        /// </summary>
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Thời gian cập nhật cuối cùng
        /// </summary>
        public DateTimeOffset LastUpdatedTime { get; set; } = DateTimeOffset.UtcNow;

        /// <summary>
        /// Thời gian xóa
        /// </summary>
        public DateTimeOffset? DeletedTime { get; set; } = DateTimeOffset.UtcNow;
        public ApplicationRoleClaims()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
