using ITC.BusinessObject.Entities;
using ITC.Repositories.Interface;
using ITC.Repositories.PaggingItems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.WalletService
{
	public class WalletTransactionService : IWalletTransactionService
	{
		private readonly IWalletTransactionRepository _walletTransactionRepository;

		public WalletTransactionService(IWalletTransactionRepository walletTransactionRepository)
		{
			_walletTransactionRepository = walletTransactionRepository;
		}

		public async Task AddWalletTransactionAsync(Guid walletId, decimal amount, string transactionType, string transactionStatus, string transactionDate, decimal transacionBalance, int? orderId)
		{
			WalletTransaction walletTransaction = new WalletTransaction
			{
				WalletId = walletId,
				Amount = amount,
				TransactionType = transactionType,
				TransactionStatus = transactionStatus,
				TransactionDate = transactionDate,
				TransactionBalance = transacionBalance.ToString(),
				OrderId = orderId
			};
			await _walletTransactionRepository.AddWalletTransactionAsync(walletTransaction);
		}

		public async Task<IEnumerable<WalletTransaction>> GetAllAsync()
		{
			return await _walletTransactionRepository.GetAllAsync();
		}
		public async Task<PaginatedList<WalletTransaction>> GetAll(int pageNumber, int pageSize)
		{
			IQueryable<WalletTransaction> walletTransaction = _walletTransactionRepository.GetAll().AsQueryable();
			return await PaginatedList<WalletTransaction>.CreateAsync(walletTransaction, pageNumber, pageSize);
		}

		public async Task<PaginatedList<WalletTransaction>> GetWalletTransactionByWalletIdAsync(Guid walletId, int pageNumber, int pageSize)
		{
			IQueryable<WalletTransaction> walletTransactions = _walletTransactionRepository.GetAll()
				.Where(wt => wt.WalletId == walletId)
				.AsQueryable();
			return await PaginatedList<WalletTransaction>.CreateAsync(walletTransactions, pageNumber, pageSize);
		}

	}
}
