using ITC.Core.Enum;
using ITC.Core.Hubs;
using ITC.Repositories.Interface;
using ITC.Services.WalletService;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobWork
{
	public class JobWorkService : IJobWorkService
	{
		private readonly IJobRepository _jobRepo;
		private readonly IWalletService _walletSvc;
		private readonly IHubContext<NotificationHub> _hub;

		public JobWorkService(
			IJobRepository jobRepo,
			IWalletService walletSvc,
			IHubContext<NotificationHub> hub)
		{
			_jobRepo = jobRepo;
			_walletSvc = walletSvc;
			_hub = hub;
		}

		/// <summary>
		/// BPDV nộp kết quả hoặc đánh dấu hoàn thành.
		/// </summary>
		public async Task SubmitWorkAsync(Guid jobId, Guid interpreterId, string? resultFileUrl)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId)
					  ?? throw new Exception("Job not found");

			if (job.SelectedInterpreterId != interpreterId)
				throw new UnauthorizedAccessException("Bạn không phải BPDV của job này");

			if (job.Status != (int)JobStatus.InProgress)
				throw new InvalidOperationException("Job chưa ở trạng thái InProgress");

			if (job.TranslationType == "Translation")
			{
				if (string.IsNullOrWhiteSpace(resultFileUrl))
					throw new ArgumentException("Kết quả dịch cần file");
				job.ResultFileUrl = resultFileUrl;
			}

			job.CompletedAt = DateTime.UtcNow;
			job.Status = (int)JobStatus.Submitted;

			await _jobRepo.SaveChangesAsync();

			// Thông báo cho Customer
			await _hub.Clients.User(job.CustomerId.ToString())
				.SendAsync("JobSubmitted", new
				{
					JobId = job.Id,
					JobTitle = job.JobTitle,
					SubmittedAt = job.CompletedAt
				});
		}

		///// <summary>
		///// Customer confirm -> tiền vào ví BPDV, job Completed.
		///// </summary>
		//public async Task ConfirmCompletionAsync(Guid jobId, Guid customerId)
		//{
		//	var job = await _jobRepo.GetJobByIdAsync(jobId)
		//			  ?? throw new Exception("Job not found");

		//	if (job.CustomerId != customerId)
		//		throw new UnauthorizedAccessException("Bạn không phải chủ job này");

		//	if (job.Status != (int)JobStatus.Submitted)
		//		throw new InvalidOperationException("Job chưa được BPDV nộp");

		//	job.Status = (int)JobStatus.Completed;
		//	job.IsPaidToInterpreter = true;

		//	await _jobRepo.SaveChangesAsync();

		//	// Chuyển tiền vào ví BPDV (giả sử TotalFee = HourlyRate * giờ / serviceFee…)
		//	if (job.TotalFee.HasValue && job.SelectedInterpreterId.HasValue)
		//	{
		//		await _walletSvc.UpdateUserWalletAsync(job.SelectedInterpreterId.Value, job.TotalFee.Value);
		//	}

		//	// Thông báo cho BPDV
		//	await _hub.Clients.User(job.SelectedInterpreterId!.Value.ToString())
		//		.SendAsync("JobCompleted", new
		//		{
		//			JobId = job.Id,
		//			JobTitle = job.JobTitle,
		//			Paid = job.TotalFee
		//		});
		//}
	}

}
