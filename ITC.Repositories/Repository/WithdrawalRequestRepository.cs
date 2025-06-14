using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using ITC.Repositories.PaggingItems;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITC.Repositories.Repository
{
    public class WithdrawalRequestRepository : IWithdrawalRequestRepository
    {
        private readonly ITCDbContext _context;

        public WithdrawalRequestRepository(ITCDbContext context)
        {
            _context = context;
        }

        public async Task<WithdrawalRequest> CreateAsync(WithdrawalRequest request)
        {
            await _context.WithdrawalRequests.AddAsync(request);
            await _context.SaveChangesAsync();
            return request;
        }

        public async Task<WithdrawalRequest> GetByIdAsync(Guid id)
        {
            return await _context.WithdrawalRequests
                .Include(w => w.Account)
                .Include(w => w.ProcessedByUser)
                .FirstOrDefaultAsync(w => w.WithdrawalRequestId == id);
        }

        public async Task<IEnumerable<WithdrawalRequest>> GetAllAsync()
        {
            return await _context.WithdrawalRequests
                .Include(w => w.Account)
                .Include(w => w.ProcessedByUser)
                .ToListAsync();
        }

        public async Task<IEnumerable<WithdrawalRequest>> GetByAccountIdAsync(Guid accountId)
        {
            return await _context.WithdrawalRequests
                .Include(w => w.Account)
                .Include(w => w.ProcessedByUser)
                .Where(w => w.AccountId == accountId)
                .ToListAsync();
        }

        public async Task<PaginatedList<WithdrawalRequest>> GetPaginatedAsync(int pageNumber, int pageSize)
        {
            IQueryable<WithdrawalRequest> query = _context.WithdrawalRequests
                .Include(w => w.Account)
                .Include(w => w.ProcessedByUser)
                .OrderByDescending(w => w.RequestDate);

            return await PaginatedList<WithdrawalRequest>.CreateAsync(query, pageNumber, pageSize);
        }

        public async Task UpdateAsync(WithdrawalRequest request)
        {
            _context.WithdrawalRequests.Update(request);
            await _context.SaveChangesAsync();
        }
    }
} 