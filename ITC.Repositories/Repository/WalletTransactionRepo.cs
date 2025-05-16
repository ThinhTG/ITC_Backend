using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace ITC.Repositories.Repository
{
	public class WalletTransactionRepo : IWalletTransactionRepository
	{
		private readonly ITCDbContext _context;

		public WalletTransactionRepo(ITCDbContext context)
		{
			_context = context;
		}

		public async Task AddWalletTransactionAsync(WalletTransaction walletTransaction)
		{
			await _context.WalletTransaction.AddAsync(walletTransaction);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<WalletTransaction>> GetAllAsync()
		{
			return await _context.Set<WalletTransaction>().ToListAsync();
		}
		public IQueryable<WalletTransaction> GetAll()
		{
			return _context.WalletTransaction.AsQueryable();
		}

		//get all wallet transaction by wallet id
		public async Task<IEnumerable<WalletTransaction>> GetWalletTransactionByWalletIdAsync(Guid walletId)
		{
			return await _context.WalletTransaction
				.Where(wt => wt.WalletId == walletId)
				.ToListAsync();
		}
	}
}
