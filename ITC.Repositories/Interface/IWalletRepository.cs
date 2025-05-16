using ITC.BusinessObject.Entities;

namespace ITC.Repositories.Interface
{
	public interface IWalletRepository
    {
        Task<Wallet> GetWalletByAccountIdAsync(Guid accountId);
        Task UpdateWalletAsync(Wallet wallet);
        Task<Wallet> CreateWallet(Wallet wallet);
        Task<Wallet> GetBalanceAccountIdAsync(Guid accountId);
    }
}
