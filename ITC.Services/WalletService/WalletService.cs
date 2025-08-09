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

		public WalletService(IWalletRepository walletRepository, IWalletTransactionService walletTransactionService, IConfiguration configuration, IPaymentService paymentService)
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
			// Lấy cấu hình thời gian
			var dateFormat = _configuration["TransactionSettings:DateFormat"] ?? "yyyy-MM-ddTHH:mm:ssZ";
			bool useUTC = bool.TryParse(_configuration["TransactionSettings:UseUTC"], out bool utc) && utc;
			var timeZoneId = _configuration["TransactionSettings:TimeZone"] ?? "UTC";

			// Lấy thời gian hiện tại
			DateTime transactionDatetime = DateTime.UtcNow;

			if (!useUTC)
			{
				try
				{
					TimeZoneInfo timeZone = TZConvert.GetTimeZoneInfo(timeZoneId);
					transactionDatetime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZone);
				}
				catch (TimeZoneNotFoundException)
				{
					throw new Exception("Invalid TimeZone");
				}
			}

			// Lấy ví
			var wallet = await _walletRepository.GetWalletByAccountIdAsync(accountId);
			if (wallet == null)
			{
				throw new Exception("Wallet not found");
			}

			// Lấy thông tin thanh toán
			var checkingPayment = await _paymentService.GetPaymentLinkInformationAsync(orderCode);

			if (checkingPayment.status == "PAID")
			{
				decimal oldBalance = wallet.Balance;
				wallet.Balance += amount;

				// Cập nhật ví trước
				await _walletRepository.UpdateWalletAsync(wallet);

				// Thêm giao dịch thành công
				await _walletTransactionService.AddWalletTransactionAsync(
					wallet.WalletId,
					amount,
					"deposit",
					"success",
					transactionDatetime,
					wallet.Balance,
					null
				);
			}
			else
			{
				// Thêm giao dịch thất bại (không cộng tiền)
				await _walletTransactionService.AddWalletTransactionAsync(
					wallet.WalletId,
					amount,
					"deposit",
					"fail",
					transactionDatetime,
					wallet.Balance,
					null
				);
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
				TransactionDate = transactionDatetime,
				TransactionBalance = wallet.Balance,
			};

			await _walletTransactionService.AddWalletTransactionAsync(wallet.WalletId, amount, "purchase", "success", transactionDatetime, wallet.Balance, orderId);
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
