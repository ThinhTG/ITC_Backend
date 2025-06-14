using ITC.BusinessObject.Entities;
using ITC.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
    public interface IWithdrawalRequestRepository
    {
        Task<WithdrawalRequest> CreateAsync(WithdrawalRequest request);
        Task<WithdrawalRequest> GetByIdAsync(Guid id);
        Task<IEnumerable<WithdrawalRequest>> GetAllAsync();
        Task<IEnumerable<WithdrawalRequest>> GetByAccountIdAsync(Guid accountId);
        Task<PaginatedList<WithdrawalRequest>> GetPaginatedAsync(int pageNumber, int pageSize);
        Task UpdateAsync(WithdrawalRequest request);
    }
} 