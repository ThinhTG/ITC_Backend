using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.WalletService
{
	public interface IWalletService
	{
		Task<Wallet> GetWalletByAccountId(Guid accountId);
		Task AddMoneyToWalletAsync(Guid accountId, decimal amount, int orderCode);
		Task<bool> UseWalletForPurchaseAsync(Guid accountId, decimal amount, int? orderId);
		Task<bool> UpdateUserWalletAsync(Wallet updatedWallet);
	}
}
