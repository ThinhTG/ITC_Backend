using ITC.Core.Utils;
using Microsoft.AspNetCore.Identity;

namespace ITC.BusinessObject.Identity
{
    public class ApplicationUserTokens : IdentityUserToken<Guid>
    {
        public DateTimeOffset CreatedTime { get; set; } = DateTimeOffset.UtcNow;

		public DateTimeOffset LastUpdatedTime { get; set; } = DateTimeOffset.UtcNow;

		public DateTimeOffset? DeletedTime { get; set; } = DateTimeOffset.UtcNow;
		public ApplicationUserTokens()
        {
            CreatedTime = CoreHelper.SystemTimeNow;
            LastUpdatedTime = CreatedTime;
        }
    }
}
