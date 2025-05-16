using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ITC.Repositories.Repository
{
	public class WalletRepository : IWalletRepository
    {
        private readonly ITCDbContext _context;

        public WalletRepository(ITCDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet> CreateWallet(Wallet wallet)
        {
            await _context.Wallets.AddAsync(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> GetWalletByAccountIdAsync(Guid accountId)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            return await _context.Wallets.FirstOrDefaultAsync(w => w.AccountId == accountId);
        }

        public async Task UpdateWalletAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet> GetBalanceAccountIdAsync(Guid accountId)
        {
            if (accountId == null)
            {
                throw new ArgumentNullException(nameof(accountId));
            }
            return await _context.Wallets.FirstOrDefaultAsync(w => w.AccountId == accountId);
        }
    }
}
