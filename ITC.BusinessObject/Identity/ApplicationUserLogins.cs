using ITC.Core.Utils;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationUserLogins : IdentityUserLogin<Guid>
    {
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

		public DateTimeOffset LastUpdatedTime { get; set; } = DateTimeOffset.UtcNow;

		public DateTimeOffset? DeletedTime { get; set; } = DateTimeOffset.UtcNow;
		public ApplicationUserLogins()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
