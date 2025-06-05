using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.Payment;
using ITC.Services.Request;
using Microsoft.AspNetCore.Identity;
using Net.payOS;
using Net.payOS.Types;

namespace ITC.Services.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly PayOS _payOS;
		//private readonly IAccountService _accountSV;
		private readonly UserManager<ApplicationUser> _accountSV;
		private readonly IWalletTransactionRepository _walletTransactionRepository;

		private readonly IWalletRepository _walletRepository;


		public PaymentService(PayOS payOS, UserManager<ApplicationUser> UserManager,IWalletRepository walletRepository, IWalletTransactionRepository walletTransactionRepository)
		{
			_payOS = payOS;
			_accountSV = UserManager;
			_walletRepository = walletRepository;
			_walletTransactionRepository = walletTransactionRepository;
		}

	
		public async Task<CreatePaymentResult> CreatePaymentLinkDepositAsync(CreateDepositLinkRequest request)
		{
			int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

			var account = await _accountSV.FindByIdAsync(request.accountId);
			if (account == null)
				throw new Exception("Account not found");

			// Nếu đã có orderCode, kiểm tra trạng thái thanh toán
			if (account.orderCode != null)
			{
				long existingOrderCode = long.Parse(account.orderCode.ToString());
				var paymentStatus = await _payOS.getPaymentLinkInformation(existingOrderCode);

				switch (paymentStatus.status)
				{
					case "PAID":
						account.orderCode = null;
						await _accountSV.UpdateAsync(account);
						throw new Exception("Deposit has already been paid");

					case "PROCESSING":
						throw new Exception("Deposit is currently processing");

					case "CANCELLED":
					case "PENDING":
						account.orderCode = null;
						await _accountSV.UpdateAsync(account);
						break;
					case "EXPIRED":
						// Cho phép tạo link mới nếu hết hạn hoặc hủy
						account.orderCode = null;
						await _accountSV.UpdateAsync(account);
						break;

					default:
						throw new Exception($"Unhandled payment status: {paymentStatus.status}");
				}
			}

			// Tạo dữ liệu thanh toán
			var item = new ItemData(request.accountId, 1, request.price);
			string description = $"Deposit {request.price}";
			long expiredAt = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();

			var paymentData = new PaymentData(
				orderCode,
				request.price,
				description,
				new List<ItemData> { item },
				request.cancelUrl,
				request.returnUrl,
				null, null, null, null, null,
				expiredAt
			);

			var createdLink = await _payOS.createPaymentLink(paymentData);

			// Gán orderCode mới và lưu lại
			account.orderCode = orderCode;
			await _accountSV.UpdateAsync(account);

			return createdLink;
		}


		public async Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderCode)
		{
			var checkOrderCode = long.Parse(orderCode.ToString());
			var response = await _payOS.getPaymentLinkInformation(checkOrderCode);
			return response;
		}

		public async Task ConfirmWebhookAsync(string webhookUrl)
		{
			await _payOS.confirmWebhook(webhookUrl);
		}

		public WebhookData VerifyPaymentWebhookData(WebhookType webhookType)
		{
			return _payOS.verifyPaymentWebhookData(webhookType);
		}


		public async Task<PaymentResult> ProcessWalletPaymentAsync(Guid customerId, decimal amount, Guid jobId)
		{
			// Lấy ví của khách
			var wallet = await _walletRepository.GetWalletByAccountIdAsync(customerId);
			if (wallet == null)
			{
				return PaymentResult.Fail("Wallet not found.");
			}

			// Kiểm tra số dư
			if (wallet.Balance < amount)
			{
				return PaymentResult.Fail("Insufficient balance.");
			}

			// Trừ tiền
			wallet.Balance -= amount;
			await _walletRepository.UpdateWalletAsync(wallet);

			// Ghi transaction
			var transaction = new WalletTransaction
			{
				WalletTransactionId = Guid.NewGuid(),
				WalletId = wallet.WalletId,
				Amount = -amount,
				TransactionType = "PAY_INTERPRETER", // Hoặc dùng enum/hằng
				TransactionStatus = "SUCCESS",       // Hoặc dùng enum/hằng
				TransactionDate = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
				TransactionBalance = wallet.Balance.ToString("F2"), // Đảm bảo 2 chữ số thập phân
			};

			await _walletTransactionRepository.AddWalletTransactionAsync(transaction);


			return PaymentResult.Success();
		}

		
	}
}
