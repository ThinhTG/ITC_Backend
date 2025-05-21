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
		Task<CreatePaymentResult> CreatePaymentLinkAsync(CreatePaymentLinkRequest request);
		Task<CreatePaymentResult> CreatePaymentLinkDepositAsync(CreateDepositLinkRequest request);
		//Task<CreatePaymentResult> CreatePaymentLinkMBAsync(CreatePaymentLinkRequestMB request);
		//Task<CreatePaymentResult> CreatePaymentLinkDepositMBAsync(CreatePaymentLinkRequestMBV2 request);
		Task<PaymentLinkInformation> GetPaymentLinkInformationAsync(int orderCode);
		Task ConfirmWebhookAsync(string webhookUrl);
		WebhookData VerifyPaymentWebhookData(WebhookType webhookType);
	}
}
