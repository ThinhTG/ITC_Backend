using ITC.BusinessObject.Entities;
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
		private readonly IWalletTransactionRepository _walletTransactionRepo;

		public JobWorkService(
			IJobRepository jobRepo,
			IWalletService walletSvc,
			IHubContext<NotificationHub> hub,
			IWalletTransactionRepository walletTransactionRepository)
		{
			_jobRepo = jobRepo;
			_walletSvc = walletSvc;
			_hub = hub;
			_walletTransactionRepo = walletTransactionRepository;
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

		public async Task ConfirmCompletionAsync(Guid jobId, Guid customerId)
		{
			//  Lấy Job kèm JobApplies & những thông tin cần
			var job = await _jobRepo.GetJobByIdAsync(jobId)
					   ?? throw new Exception("Job not found");

			if (job.CustomerId != customerId)
				throw new UnauthorizedAccessException("Bạn không phải chủ job này");

			if (job.Status != (int)JobStatus.Submitted)
				throw new InvalidOperationException("Job chưa ở trạng thái Submitted");

			var wallet = await _walletSvc.GetWalletByAccountId(job.SelectedInterpreterId!.Value);

			//  Cập nhật trạng thái Job → Completed (5) + CompletedAt
			job.Status = (int)JobStatus.Completed;
			job.CompletedAt = DateTime.UtcNow;
			job.IsPaidToInterpreter = true;

			//  Đánh dấu JobApply của BPDV được chọn → Done (3)
			var chosenApply = job.Applications
								 .FirstOrDefault(a => a.InterpreterId == job.SelectedInterpreterId);
			if (chosenApply != null)
				chosenApply.Status = "Done";

			//  Tính chênh lệch Deadline 
			if (job.Deadline.HasValue)
				job.CompletionOffsetMinutes =
					(int)(job.CompletedAt!.Value - job.Deadline.Value).TotalMinutes; 

			//  Ghi nhận giao dịch ví (WalletTransaction) + cộng tiền
			if (job.TotalFee > 0 && job.SelectedInterpreterId.HasValue)
			{
				var tx = new WalletTransaction
				{
					WalletTransactionId = Guid.NewGuid(),
					Amount = job.HourlyRate.Value,
					TransactionType = "Job Payment",  // Enum tuỳ bạn định nghĩa
													  //JobId = job.Id,
					CreateAt = DateTime.UtcNow,
					Description = $"Thanh toán job \"{job.JobTitle}\""
				};

				await _walletTransactionRepo.AddWalletTransactionAsync(tx);   // tạo record

				wallet.Balance += job.HourlyRate.Value;  // cộng tiền vào ví
				await _walletSvc.UpdateUserWalletAsync(wallet);  // + tiền vào ví
			}

			await _jobRepo.SaveChangesAsync();   // tất cả thay đổi trong một transaction

			//  Thông báo realtime qua SignalR cho BPDV
			await _hub.Clients.User(job.SelectedInterpreterId!.Value.ToString())
				.SendAsync("JobCompleted", new
				{
					JobId = job.Id,
					JobTitle = job.JobTitle,
					Paid = job.TotalFee,
					OffsetMinute = job.CompletionOffsetMinutes      // cho biết sớm/trễ
				});

			//// (tuỳ ý) Thông báo cho Admin / Customer
			//await _notificationSvc.CreateAsync(new Notification
			//{
			//	Id = Guid.NewGuid(),
			//	UserId = job.SelectedInterpreterId!.Value,
			//	Title = "Bạn đã nhận thanh toán",
			//	Content = $"Khách hàng đã xác nhận hoàn thành job \"{job.JobTitle}\".",
			//	CreatedAt = DateTime.UtcNow,
			//	IsRead = false,
			//	Link = $"/jobs/{job.Id}"
			//});
		}

	}

}
