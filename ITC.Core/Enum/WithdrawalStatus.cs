using System;

namespace ITC.Core.Enum
{
    /// <summary>
    /// Trạng thái của yêu cầu rút tiền
    /// </summary>
    public enum WithdrawalStatus
    {
        Pending = 0,                    // BPDV gửi request withdraw
        WaitingForConfirmation = 1,     // Staff đã chuyển tiền, chờ BPDV xác nhận
        Completed = 2                   // BPDV đã xác nhận nhận được tiền
    }
} 