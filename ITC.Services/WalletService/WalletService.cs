using ITC.BusinessObject.Entities;
using ITC.Repositories.Interface;
using ITC.Services.PaymentService;
using Microsoft.Extensions.Configuration;
using System.Globalization;
using TimeZoneConverter;

namespace ITC.Services.WalletService
{
	public class WalletService : IWalletService
	{
		private readonly IWalletRepository _walletRepository;
		private readonly IWalletTransactionService _walletTransactionService;
		private readonly IPaymentService _paymentService;
		private readonly IConfiguration _configuration;

		public WalletService(IWalletRepository walletRepository, IWalletTransactionService walletTransactionService,IPaymentService paymentService, IConfiguration configuration)
		{
			_walletRepository = walletRepository;
			_walletTransactionService = walletTransactionService;
			_configuration = configuration;
			_paymentService = paymentService;
		}

		public async Task<Wallet> CreateWallet(Wallet wallet)
		{
			try
			{
				var newWallet = await _walletRepository.CreateWallet(wallet);
				return newWallet;
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<Wallet> GetWalletByAccountId(Guid accountId)
		{
			var wallet = await _walletRepository.GetWalletByAccountIdAsync(accountId);
			if (wallet == null)
			{
				throw new Exception("Wallet not found");
			}

			return wallet;
		}

		public async Task AddMoneyToWalletAsync(Guid accountId, decimal amount, int orderCode)
		{
			var dateFormat = _configuration["TransactionSettings:DateFormat"] ?? "yyyy-MM-ddTHH:mm:ssZ";
			bool useUTC = bool.TryParse(_configuration["TransactionSettings:UseUTC"], out bool utc) && utc;
			var timeZoneId = _configuration["TransactionSettings:TimeZone"] ?? "UTC";
			DateTime transactionDatetime = DateTime.UtcNow; // Default to UTC

			var wallet = await _walletRepository.GetWalletByAccountIdAsync(accountId);
			if (wallet == null)
			{
				throw new Exception("Wallet not found");
			}

			//check payment status
			var checkingPayment = await _paymentService.GetPaymentLinkInformationAsync(orderCode);
			if (checkingPayment.status == "PAID")
			{
				wallet.Balance += amount;
				await _walletRepository.UpdateWalletAsync(wallet);
			}

			wallet.Balance += amount;


			if (!useUTC)
			{
				try
				{
					// Convert UTC time to specified TimeZone
					TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
					transactionDatetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				}
				catch (TimeZoneNotFoundException)
				{
					throw new Exception("Invalid TimeZone");
				}
			}

			try
			{

				await _walletTransactionService.AddWalletTransactionAsync(wallet.WalletId, amount, "deposit", "success", transactionDatetime.ToString(dateFormat, CultureInfo.InvariantCulture), wallet.Balance, null);
			}
			catch (Exception ex)
			{
				throw new Exception(ex.Message);
			}
		}

		public async Task<bool> UseWalletForPurchaseAsync(Guid accountId, decimal amount, int? orderId)
		{
			var dateFormat = _configuration["TransactionSettings:DateFormat"] ?? "yyyy-MM-ddTHH:mm:ssZ";
			bool useUTC = bool.TryParse(_configuration["TransactionSettings:UseUTC"], out bool utc) && utc;
			var timeZoneId = _configuration["TransactionSettings:TimeZone"] ?? "UTC";
			DateTime transactionDatetime = DateTime.UtcNow; // Default to UTC

			var wallet = await _walletRepository.GetWalletByAccountIdAsync(accountId);
			if (wallet == null)
			{
				throw new Exception("Wallet not found");
			}
			if (wallet.Balance < amount)
			{
				return false;
			}
			wallet.Balance -= amount;
			await _walletRepository.UpdateWalletAsync(wallet);

			if (!useUTC)
			{
				try
				{
					// Convert UTC time to specified TimeZone
					TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
					transactionDatetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				}
				catch (TimeZoneNotFoundException)
				{
					throw new Exception("Invalid TimeZone");
				}
			}

			var walletTransaction = new WalletTransaction
			{
				WalletId = wallet.WalletId,
				Amount = amount,
				TransactionType = "Debit",
				TransactionStatus = "Success",
				TransactionDate = transactionDatetime.ToString(dateFormat, CultureInfo.InvariantCulture),
				TransactionBalance = wallet.Balance.ToString(),
				OrderId = orderId
			};

			await _walletTransactionService.AddWalletTransactionAsync(wallet.WalletId, amount, "purchase", "success", transactionDatetime.ToString(dateFormat, CultureInfo.InvariantCulture), wallet.Balance, orderId);
			return true;
		}

		public async Task<bool> UpdateUserWalletAsync(Wallet updatedWallet)
		{
			if (updatedWallet == null)
				throw new ArgumentNullException(nameof(updatedWallet), "Wallet cannot be null.");

			var wallet = await _walletRepository.GetWalletByAccountIdAsync(updatedWallet.AccountId);
			if (wallet == null)
				throw new Exception("Wallet not found");

			// Cập nhật số dư và ngày cập nhật
			wallet.Balance = updatedWallet.Balance;

			await _walletRepository.UpdateWalletAsync(wallet);

			return true;
		}
	}
}
