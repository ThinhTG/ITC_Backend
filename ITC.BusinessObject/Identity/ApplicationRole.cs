    using ITC.Core.Utils;
    using Microsoft.AspNetCore.Identity;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace ITC.BusinessObject.Identity
    {
        public class ApplicationRole : IdentityRole<Guid>
        {

            /// <summary>
            /// Tên vai trò
            /// </summary>
            public string? FullName { get; set; }

            /// <summary>
            /// Thời gian tạo
            /// </summary>
            public DateTimeOffset CreatedTime { get; set; }

            /// <summary>
            /// Thời gian lần cuối cập nhật
            /// </summary>
            public DateTimeOffset LastUpdatedTime { get; set; }

            /// <summary>
            /// Thời gian xóa
            /// </summary>
            public DateTimeOffset? DeletedTime { get; set; }
            public ApplicationRole()
            {
                CreatedTime = CoreHelper.SystemTimeNow;
                LastUpdatedTime = CreatedTime;
                ConcurrencyStamp = Guid.NewGuid().ToString();
            }
        }
    }
