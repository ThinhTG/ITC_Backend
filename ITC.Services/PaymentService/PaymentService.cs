using ITC.BusinessObject.Identity;
using ITC.Core.Contracts;
using ITC.Services.OrderService;
using ITC.Services.Request;
using Microsoft.AspNetCore.Identity;
using Net.payOS;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly PayOS _payOS;
		private readonly IOrderService _orderSV;
		//private readonly IAccountService _accountSV;
		private readonly UserManager<ApplicationUser> _accountSV;


		public PaymentService(PayOS payOS, IOrderService orderService, UserManager<ApplicationUser> UserManager)
		{
			_payOS = payOS;
			_orderSV = orderService;
			_accountSV = UserManager;
		}

		public async Task<CreatePaymentResult> CreatePaymentLinkAsync(CreatePaymentLinkRequest request)
		{
			var order = await _orderSV.GetByIdAsync(request.orderId);
			if (order.OrderCode != null)
			{
				var checkOrderCode = long.Parse(order.OrderCode.ToString());
				var checking = await _payOS.getPaymentLinkInformation(checkOrderCode);
				if (checking.status == "CANCELLED" || checking.status == "PENDING")
				{
					order.OrderCode = null;
					await _orderSV.UpdateAsync(order);
				}

				if (checking.status == "PAID")
				{
					order.PaymentConfirmed = true;
					await _orderSV.UpdateAsync(order);
					throw new Exception("Order has been paid");
				}

				if (checking.status == "PROCESSING")
				{
					throw new Exception("Order is processing");
				}
			}
			int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
			ItemData item = new ItemData(request.orderId.ToString(), 1, request.price);
			var descriptions = request.description = $"Payment {request.orderId}";
			List<ItemData> items = new List<ItemData> { item };
			var expiredAt = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();
			PaymentData paymentDataPayment = new PaymentData(orderCode, request.price, descriptions, items, request.cancelUrl, request.returnUrl, null, null, null, null, null, expiredAt);
			try
			{
				var createdLink = await _payOS.createPaymentLink(paymentDataPayment);

				order.OrderCode = orderCode;
				await _orderSV.UpdateAsync(order);
				return createdLink;

			}
			catch (Exception ex)
			{
				throw new Exception();
			}


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



		//public async Task<CreatePaymentResult> CreatePaymentLinkMBAsync(CreatePaymentLinkRequestMB request)
		//{
		//	var order = await _orderSV.GetByIdAsync(request.orderId);
		//	if (order.OrderCode != null)
		//	{
		//		var checkOrderCode = long.Parse(order.OrderCode.ToString());
		//		var checking = await _payOS.getPaymentLinkInformation(checkOrderCode);
		//		if (checking.status == "CANCELLED" || checking.status == "PENDING")
		//		{
		//			order.OrderCode = null;
		//			await _orderSV.UpdateAsync(order);
		//		}

		//		if (checking.status == "PAID")
		//		{
		//			order.PaymentConfirmed = true;
		//			await _orderSV.UpdateAsync(order);
		//			throw new Exception("Order has been paid");
		//		}

		//		if (checking.status == "PROCESSING")
		//		{
		//			throw new Exception("Order is processing");
		//		}
		//	}
		//	int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));
		//	ItemData item = new ItemData(request.orderId.ToString(), 1, request.price);
		//	var descriptions = request.description = $"Payment {request.orderId}";
		//	List<ItemData> items = new List<ItemData> { item };
		//	var expiredAt = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();
		//	PaymentData paymentDataPayment = new PaymentData(orderCode, request.price, descriptions, items, request.cancelUrl, request.returnUrl, null, null, null, null, null, expiredAt);
		//	try
		//	{
		//		var createdLink = await _payOS.createPaymentLink(paymentDataPayment);

		//		order.OrderCode = orderCode;
		//		await _orderSV.UpdateAsync(order);
		//		return createdLink;

		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception();
		//	}


		//}

		//public async Task<CreatePaymentResult> CreatePaymentLinkDepositMBAsync(CreatePaymentLinkRequestMBV2 request)
		//{
		//	int orderCode = int.Parse(DateTimeOffset.Now.ToString("ffffff"));

		//	var parseID = Guid.Parse(request.accountId);
		//	var account = await _accountSV.GetByIdAsync(request.accountId);
		//	if (account.orderCode != null)
		//	{
		//		var checkOrderCode = long.Parse(account.orderCode.ToString());
		//		var checking = await _payOS.getPaymentLinkInformation(checkOrderCode);
		//		if (checking.status == "CANCELLED" || checking.status == "PENDING")
		//		{
		//			var newacount = new UserRequestAndResponse.UpdateOrderCodeRequest
		//			{
		//				orderCode = null
		//			};
		//			await _accountSV.UpdateAsync(parseID, newacount);
		//		}
		//		if (checking.status == "PAID")
		//		{
		//			var newacount = new UserRequestAndResponse.UpdateOrderCodeRequest
		//			{
		//				orderCode = null
		//			};
		//			await _accountSV.UpdateAsync(parseID, newacount);
		//			throw new Exception("Deposit has been paid");
		//		}
		//		if (checking.status == "PROCESSING")
		//		{
		//			throw new Exception("Deposit is processing");
		//		}
		//	}

		//	ItemData item = new ItemData(request.accountId, 1, request.price);
		//	var descriptions = request.description = $"Deposit {request.price}";
		//	List<ItemData> items = new List<ItemData> { item };
		//	var expiredAt = DateTimeOffset.Now.AddMinutes(15).ToUnixTimeSeconds();
		//	PaymentData paymentDataPayment = new PaymentData(orderCode, request.price, descriptions, items, request.cancelUrl, request.returnUrl, null, null, null, null, null, expiredAt);
		//	try
		//	{
		//		var createdLink = await _payOS.createPaymentLink(paymentDataPayment);

		//		account.orderCode = orderCode;
		//		var newacount = new UserRequestAndResponse.UpdateOrderCodeRequest
		//		{
		//			orderCode = orderCode
		//		};
		//		await _accountSV.UpdateAsync(parseID, newacount);
		//		return createdLink;

		//	}
		//	catch (Exception ex)
		//	{
		//		throw new Exception();
		//	}

		//}

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
	}
}
