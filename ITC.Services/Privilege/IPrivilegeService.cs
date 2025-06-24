using ITC.Core.Enum;
using System.Threading.Tasks;
using System;

namespace ITC.Services.Privilege
{
    public interface IPrivilegeService
    {
        Task<PrivilegeLevel> GetUserPrivilegeLevelAsync(Guid userId);
    }
} 