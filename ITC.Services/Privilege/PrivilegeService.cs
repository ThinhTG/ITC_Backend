using ITC.Core.Enum;
using ITC.Repositories.Interface;
using System.Threading.Tasks;
using System;

namespace ITC.Services.Privilege
{
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IUserSubscriptionRepository _userSubRepo;

        public PrivilegeService(IUserSubscriptionRepository userSubRepo)
        {
            _userSubRepo = userSubRepo;
        }

        public async Task<PrivilegeLevel> GetUserPrivilegeLevelAsync(Guid userId)
        {
            var activeSub = await _userSubRepo.GetActiveSubscriptionAsync(userId);

            if (activeSub == null || activeSub.SubscriptionPlan == null)
            {
                return PrivilegeLevel.NoSubscription;
            }

            // Mapping từ tên gói sang cấp độ đặc quyền
            switch (activeSub.SubscriptionPlan.Name.ToLower())
            {
                case "gói 1":
                case "basic":
                    return PrivilegeLevel.Basic;

                case "gói 2":
                case "premium":
                    return PrivilegeLevel.Premium;

                case "gói 3":
                case "vip":
                    return PrivilegeLevel.Vip;

                default:
                    return PrivilegeLevel.NoSubscription;
            }
        }
    }
} 