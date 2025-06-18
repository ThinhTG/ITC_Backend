using ITC.BusinessObject.Request;
using ITC.Core.Base;
using ITC.Core.Constants;
using ITC.Core.Enum;
using ITC.Services.JobApplyService;
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

		private readonly IJobApplicationService _jobApplicationService;

		public PaymentController(IPaymentService paymentService,IJobService jobService, IJobApplicationService jobApplicationService)
		{
			_paymentService = paymentService;
			_jobService = jobService;	
			_jobApplicationService = jobApplicationService;
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
				// Get job and verify it exists
				var job = await _jobService.GetJobDetailsDtoByIdAsync(request.JobId);
				if (job == null)
				{
					return NotFound(new BaseResponse<string>(
						StatusCodeHelper.NotFound,
						ResponseCodeConstants.NOT_FOUND,
						"Job not found"));
				}

				// Get the specific application for this interpreter
				var applications = await _jobApplicationService.GetApplicationsForJobAsync(request.JobId);
				var application = applications.FirstOrDefault(a => a.InterpreterId == request.InterpreterId);
				
				if (application == null)
				{
					return NotFound(new BaseResponse<string>(
						StatusCodeHelper.NotFound,
						ResponseCodeConstants.NOT_FOUND,
						"Interpreter application not found"));
				}

				if (application.WorkStatus != (int)InterpreterWorkStatus.AwaitingPayment)
				{
					return BadRequest(new BaseResponse<string>(
						StatusCodeHelper.BadRequest,
						ResponseCodeConstants.BADREQUEST,
						$"Interpreter is not in awaiting payment status. Current status: {application.WorkStatus}"));
				}

				// Process payment
				var result = await _paymentService.ProcessWalletPaymentAsync(request.CustomerId, request.Amount, request.JobId);
				if (!result.IsSuccess)
				{
					return BadRequest(new BaseResponse<string>(
						StatusCodeHelper.BadRequest,
						ResponseCodeConstants.FAILED,
						result.ErrorMessage ?? "Payment failed"));
				}

				// Update application payment status
				application.WorkStatus = (int)InterpreterWorkStatus.Paid;
				application.IsPaid = true;
				application.IndividualFee = request.Amount;
				application.PaidAt = DateTimeOffset.UtcNow;
				application.LastUpdatedAt = DateTimeOffset.UtcNow;

				// Save changes to database
				await _jobApplicationService.SaveChangesAsync();

				// Note: Job status is not updated here since payment is per interpreter
				// Job status will be updated when interpreters start working

				return Ok(BaseResponse<string>.OkDataResponse($"Payment successful. Interpreter {request.InterpreterId} status updated to Paid."));
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
		/// Debug endpoint để kiểm tra trạng thái thanh toán
		/// </summary>
		/// <param name="jobId"></param>
		/// <param name="interpreterId"></param>
		/// <returns></returns>
		[HttpGet("debug/{jobId}/{interpreterId}")]
		public async Task<IActionResult> DebugPayment(Guid jobId, Guid interpreterId)
		{
			try
			{
				var job = await _jobService.GetJobDetailsDtoByIdAsync(jobId);
				if (job == null)
				{
					return NotFound(new { message = "Job not found" });
				}

				var applications = await _jobApplicationService.GetApplicationsForJobAsync(jobId);
				var application = applications.FirstOrDefault(a => a.InterpreterId == interpreterId);
				
				if (application == null)
				{
					return NotFound(new { message = "Interpreter application not found" });
				}

				return Ok(new
				{
					JobId = jobId,
					InterpreterId = interpreterId,
					ApplicationStatus = application.ApplicationStatus,
					WorkStatus = application.WorkStatus,
					IsPaid = application.IsPaid,
					IndividualFee = application.IndividualFee,
					PaidAt = application.PaidAt,
					LastUpdatedAt = application.LastUpdatedAt
				});
			}
			catch (Exception ex)
			{
				return StatusCode(500, new { message = ex.Message, details = ex.StackTrace });
			}
		}

	}

}
