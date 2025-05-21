using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface IWalletTransactionRepository
	{
		Task AddWalletTransactionAsync(WalletTransaction walletTransaction);
		Task<IEnumerable<WalletTransaction>> GetWalletTransactionByWalletIdAsync(Guid walletId);
		Task<IEnumerable<WalletTransaction>> GetAllAsync();
		IQueryable<WalletTransaction> GetAll();
	}
}
