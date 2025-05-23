using Azure;
using ITC.Core.Base;
using ITC.Core.Constants;
using ITC.Services.OrderService;
using ITC.Services.PaymentService;
using ITC.Services.Request;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[Route("api/payments")]
	[ApiController]
	public class PaymentController : ControllerBase
	{
		private readonly IPaymentService _paymentService;
		private readonly IOrderService _orderService;


		public PaymentController(IPaymentService paymentService, IOrderService orderService)
		{
			_paymentService = paymentService;
			_orderService = orderService;

		}


		/// <summary>
		/// Tao Link PayOS thanh toan khi Sellect BPDV
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		[HttpPost("createPayment")]
		public async Task<IActionResult> CreatePayment([FromBody] CreatePaymentLinkRequest body)
		{
			if (body == null)
				return BadRequest(BaseResponse<string>.OkResponse("Request body is null"));

			var order = await _orderService.GetByIdAsync(body.orderId);
			var userId = order.CustomerId;

			try
			{
				var paymentLink = await _paymentService.CreatePaymentLinkAsync(body);
				return Ok(BaseResponse<string>.OkDataResponse(paymentLink));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new BaseResponse<string>(StatusCodeHelper.ServerError, ResponseCodeConstants.INTERNAL_SERVER_ERROR, "Internal Server Error"));
			}
		}

		/// <summary>
		/// Tao link PayOS nap tien vao vi
		/// </summary>
		/// <param name="body"></param>
		/// <returns></returns>
		[HttpPost("createDeposit")]
		public async Task<IActionResult> CreateDeposit([FromBody] CreateDepositLinkRequest body)
		{
			if (body == null)
			{
				return BadRequest(new BaseResponse<string>(
					StatusCodeHelper.BadRequest,
					ResponseCodeConstants.BADREQUEST,
					"Request body is null"));
			}

			try
			{
				var paymentLink = await _paymentService.CreatePaymentLinkDepositAsync(body);
				return Ok(BaseResponse<string>.OkDataResponse(paymentLink));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new BaseResponse<string>(
						StatusCodeHelper.ServerError,
						ResponseCodeConstants.INTERNAL_SERVER_ERROR,
						"Internal Server Error"));
			}
		}


		/// <summary>
		/// Lấy thông tin thanh toán theo orderCode
		/// </summary>
		/// <param name="orderCode">Order Code duoc dung de xac thuc voi ben PayOS</param>
		/// <returns></returns>
		[HttpGet("{orderCode}")]
		public async Task<IActionResult> GetPayment(int orderCode)
		{
			try
			{
				var paymentInfo = await _paymentService.GetPaymentLinkInformationAsync(orderCode);

				// Nếu không tìm thấy hoặc trạng thái rỗng
				if (paymentInfo == null || string.IsNullOrWhiteSpace(paymentInfo.status))
				{
					return NotFound(new BaseResponse<string>(
						StatusCodeHelper.NotFound,
						ResponseCodeConstants.NOT_FOUND,
						"Payment information not found or invalid."));
				}

				return Ok(BaseResponse<object>.OkDataResponse(paymentInfo));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				return StatusCode(StatusCodes.Status500InternalServerError,
					new BaseResponse<string>(
						StatusCodeHelper.ServerError,
						ResponseCodeConstants.INTERNAL_SERVER_ERROR,
						"Internal Server Error"));
			}
		}
	}

	}
