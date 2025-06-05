using ITC.Services.DTOs.Payment;
using ITC.Services.Request;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.PaymentService
{
	public interface IPaymentService
	{
		Task<CreatePaymentResult> CreatePaymentLinkDepositAsync(CreateDepositLinkRequest request);
		//Task<CreatePaymentResult> CreatePaymentLinkMBAsync(CreatePaymentLinkRequestMB request);
		//Task<CreatePaymentResult> CreatePaymentLinkDepositMBAsync(CreatePaymentLinkRequestMBV2 request);
		Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderCode);
		Task ConfirmWebhookAsync(string webhookUrl);
		WebhookData VerifyPaymentWebhookData(WebhookType webhookType);

		Task<PaymentResult> ProcessWalletPaymentAsync(Guid customerId, decimal amount, Guid jobId);

	}
}
