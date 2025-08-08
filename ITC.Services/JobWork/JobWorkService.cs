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
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using ITC.Core.Utils;
using ITC.Services.Privilege;

namespace ITC.Services.JobWork
{
	public class JobWorkService : IJobWorkService
	{
		private readonly IJobRepository _jobRepo;
		private readonly IWalletService _walletSvc;
		private readonly IHubContext<NotificationHub> _hub;
		private readonly IWalletTransactionRepository _walletTransactionRepo;
		private readonly IPrivilegeService _privilegeService;

		public JobWorkService(
			IJobRepository jobRepo,
			IWalletService walletSvc,
			IHubContext<NotificationHub> hub,
			IWalletTransactionRepository walletTransactionRepository,
			IPrivilegeService privilegeService)
		{
			_jobRepo = jobRepo;
			_walletSvc = walletSvc;
			_hub = hub;
			_walletTransactionRepo = walletTransactionRepository;
			_privilegeService = privilegeService;
		}

		/// <summary>
		/// BPDV bắt đầu làm việc - chuyển trạng thái từ Paid sang InProgress ( WorkStatus)
		/// </summary>
		public async Task StartWorkAsync(Guid jobId, Guid interpreterId)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId)
					  ?? throw new Exception($"Job not found with ID: {jobId}");

			// Debug: Kiểm tra xem job có applications không
			if (job.Applications == null || !job.Applications.Any())
			{
				throw new Exception($"Job {jobId} has no applications");
			}

			var application = job.Applications.FirstOrDefault(a => a.InterpreterId == interpreterId);
			if (application == null)
			{
				var availableInterpreterIds = string.Join(", ", job.Applications.Select(a => a.InterpreterId));
				throw new Exception($"Interpreter {interpreterId} not found in job {jobId}. Available interpreters: {availableInterpreterIds}");
			}

			if (application.WorkStatus != (int)InterpreterWorkStatus.Paid)
				throw new InvalidOperationException($"Interpreter {interpreterId} chưa được thanh toán hoặc không ở trạng thái Paid. Current status: {application.WorkStatus}");

			// Chuyển trạng thái sang InProgress cho BPDV này
			application.WorkStatus = (int)InterpreterWorkStatus.InProgress;
			application.StartedAt = TimeHelper.GetVietnameseTime();
			application.LastUpdatedAt = TimeHelper.GetVietnameseTime();

			// Cập nhật trạng thái Job nếu đây là BPDV đầu tiên bắt đầu làm việc
			if (job.Status == (int)JobStatus.Recruiting || job.Status == (int)JobStatus.FullyRecruited)
			{
				job.Status = (int)JobStatus.InProgress;
			}

			await _jobRepo.SaveChangesAsync();

			// Thông báo cho Customer
			await _hub.Clients.User(job.CustomerId.ToString())
				.SendAsync("JobStarted", new
				{
					JobId = job.Id,
					JobTitle = job.JobTitle,
					InterpreterId = interpreterId,
					StartedAt = TimeHelper.GetVietnameseTime()
				});
		}

		/// <summary>
		/// BPDV nộp kết quả hoặc đánh dấu hoàn thành.
		/// </summary>
		public async Task SubmitWorkAsync(Guid jobId, Guid interpreterId, string? resultFileUrl)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId)
					  ?? throw new Exception($"Job not found with ID: {jobId}");

			if (job.Applications == null || !job.Applications.Any())
			{
				throw new Exception($"Job {jobId} has no applications");
			}

			var application = job.Applications.FirstOrDefault(a => a.InterpreterId == interpreterId);
			if (application == null)
			{
				var availableInterpreterIds = string.Join(", ", job.Applications.Select(a => a.InterpreterId));
				throw new Exception($"Interpreter {interpreterId} not found in job {jobId}. Available interpreters: {availableInterpreterIds}");
			}

			if (application.WorkStatus != (int)InterpreterWorkStatus.InProgress)
				throw new InvalidOperationException($"Interpreter {interpreterId} chưa ở trạng thái InProgress. Current status: {application.WorkStatus}");

			// Debug logging
			Console.WriteLine($"SubmitWorkAsync Debug:");
			Console.WriteLine($"JobId: {jobId}");
			Console.WriteLine($"InterpreterId: {interpreterId}");
			Console.WriteLine($"JobType: {job.TranslationType}");
			Console.WriteLine($"ResultFileUrl: {resultFileUrl}");
			Console.WriteLine($"Current IndividualResultFileUrl: {application.IndividualResultFileUrl}");

			// Only save file URL for Written jobs
			if (job.TranslationType == "Written")
			{
				if (string.IsNullOrWhiteSpace(resultFileUrl))
					throw new ArgumentException("Kết quả dịch Written cần file");
				application.IndividualResultFileUrl = resultFileUrl;
				Console.WriteLine($"Setting IndividualResultFileUrl to: {resultFileUrl}");
			}

			application.CompletedAt = TimeHelper.GetVietnameseTime();
			application.WorkStatus = (int)InterpreterWorkStatus.Submitted;
			application.LastUpdatedAt = TimeHelper.GetVietnameseTime();

			await _jobRepo.SaveChangesAsync();
			Console.WriteLine($"Saved to database. Final IndividualResultFileUrl: {application.IndividualResultFileUrl}");

			// Thông báo cho Customer
			await _hub.Clients.User(job.CustomerId.ToString())
				.SendAsync("JobSubmitted", new
				{
					JobId = job.Id,
					JobTitle = job.JobTitle,
					InterpreterId = interpreterId,
					SubmittedAt = TimeHelper.GetVietnameseTime()
				});
		}

		public async Task ConfirmCompletionAsync(Guid jobId, Guid customerId)
		{
			//  Lấy Job kèm JobApplies & những thông tin cần
			var job = await _jobRepo.GetJobByIdAsync(jobId)
					   ?? throw new Exception("Job not found");

			if (job.CustomerId != customerId)
				throw new UnauthorizedAccessException("Bạn không phải chủ job này");

			// Tìm tất cả các application đã submitted
			var submittedApplications = job.Applications?.Where(a => a.WorkStatus == (int)InterpreterWorkStatus.Submitted).ToList() ?? new List<JobApplication>();
			
			if (!submittedApplications.Any())
				throw new InvalidOperationException("Không có BPDV nào đã nộp kết quả");

			// Xác nhận hoàn thành cho tất cả BPDV đã submitted
			foreach (var application in submittedApplications)
			{
				application.WorkStatus = (int)InterpreterWorkStatus.Completed;
				application.LastUpdatedAt = TimeHelper.GetVietnameseTime();

				// Tính chênh lệch Deadline 
				if (job.Deadline.HasValue && application.CompletedAt.HasValue)
					application.CompletionOffsetMinutes = (int)(application.CompletedAt.Value - job.Deadline.Value).TotalMinutes;

				// Ghi nhận giao dịch ví (WalletTransaction) + cộng tiền cho BPDV này (đã trừ phí dịch vụ)
				if (application.IndividualFee > 0)
				{
					var wallet = await _walletSvc.GetWalletByAccountId(application.InterpreterId);
					if (wallet != null)
					{
						// Lấy phần trăm phí dịch vụ cho BPDV này
						var commissionPercent = (decimal)0.3;
						var serviceFee = application.IndividualFee.Value * commissionPercent;
						var netAmount = application.IndividualFee.Value - serviceFee;

						var tx = new WalletTransaction
						{
							WalletId = wallet.WalletId,
							WalletTransactionId = Guid.NewGuid(),
							Amount = (decimal)job.HourlyRate,
							TransactionBalance = wallet.Balance + (decimal)job.HourlyRate,
							TransactionStatus = "Completed",
							TransactionDate = TimeHelper.GetVietnameseTime(), 
							TransactionType = "Job Payment",
							CreateAt = TimeHelper.GetVietnameseTime(),
							Description = $"Thanh toán job \"{job.JobTitle}\" cho BPDV {application.InterpreterId} (đã trừ phí dịch vụ {serviceFee:N0}đ)"
						};

						wallet.Balance += (decimal)job.HourlyRate;
						await _walletTransactionRepo.AddWalletTransactionAsync(tx);

						// TODO: Ghi nhận transaction phí dịch vụ về ví hệ thống nếu cần
					}
				}
			}

			// Cập nhật trạng thái tổng thể của job
			if (job.IsAllCompleted)
			{
				job.Status = (int)JobStatus.Completed;
			}
			else if (job.TotalCompletedInterpreters > 0)
			{
				job.Status = (int)JobStatus.PartiallyCompleted;
			}

			await _jobRepo.SaveChangesAsync();
		}

	}

}
