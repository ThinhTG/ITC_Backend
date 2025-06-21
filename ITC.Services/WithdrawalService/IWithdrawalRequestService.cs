using ITC.Services.DTOs.Withdrawal;
using ITC.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITC.Services.WithdrawalService
{
    public interface IWithdrawalRequestService
    {
        Task<WithdrawalRequestDto> CreateAsync(Guid accountId, CreateWithdrawalRequestDto dto);
        Task<WithdrawalRequestDto> GetByIdAsync(Guid id);
        Task<IEnumerable<WithdrawalRequestDto>> GetByAccountIdAsync(Guid accountId);
        Task<PaginatedList<WithdrawalRequestDto>> GetAllAsync(int pageNumber, int pageSize);
        Task<WithdrawalRequestDto> UpdateStatusAsync(Guid id, UpdateWithdrawalRequestDto dto, Guid staffId);
        Task<WithdrawalRequestDto> ConfirmReceivedAsync(Guid id, Guid accountId);
        Task<bool> CancelRequestAsync(Guid requestId, Guid accountId);
    }
} 