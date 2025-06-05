using ITC.BusinessObject.Request;
using ITC.Core.Base;
using ITC.Core.Constants;
using ITC.Core.Enum;
using ITC.Services.JobService;
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
	
		private readonly IJobService _jobService;


		public PaymentController(IPaymentService paymentService,IJobService jobService)
		{
			_paymentService = paymentService;
			_jobService = jobService;	

		}

		/// <summary>
		/// Tao Link Deposit vao vi tiền của khách hàng
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
		/// Lấy Thông tin thanh toán theo OrderCode
		/// </summary>
		/// <param name="orderCode"></param>
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


		/// <summary>
		/// Thanh toán sau khi chọn thông dịch viên (trừ ví khách + cập nhật job đã thanh toán)
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		[HttpPost("pay-interpreter")]
		public async Task<IActionResult> PayInterpreter([FromBody] InterpreterPaymentRequest request)
		{
			if (request == null || request.Amount <= 0)
			{
				return BadRequest(new BaseResponse<string>(
					StatusCodeHelper.BadRequest,
					ResponseCodeConstants.BADREQUEST,
					"Invalid payment request"));
			}

			try
			{
				// Trừ tiền trong ví
				var result = await _paymentService.ProcessWalletPaymentAsync(request.CustomerId, request.Amount, request.JobId);
				if (!result.IsSuccess)
				{
					return BadRequest(new BaseResponse<string>(
						StatusCodeHelper.BadRequest,
						ResponseCodeConstants.FAILED,
						result.ErrorMessage ?? "Payment failed"));
				}

				// Cập nhật trạng thái Job
				await _jobService.UpdateJobStatusAsync(request.JobId, (int)JobStatus.Paid);

				return Ok(BaseResponse<string>.OkDataResponse("Payment successful and job updated."));
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
