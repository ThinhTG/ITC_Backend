using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.Core.Enum;
using ITC.Core.Hubs;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.JobApply;
using ITC.Services.Notification;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobApplyService
{
	public class JobApplicationService : IJobApplicationService
	{
		private readonly IJobApplicationRepository _ApplyRepository;
		private readonly IJobRepository _jobRepository;
		private readonly IHubContext<NotificationHub> _hubContext;
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly INotificationService _notificationService;


		public JobApplicationService(IJobApplicationRepository repository, INotificationService notificationService, IJobRepository jobRepository,UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> hubContext)
		{
			_ApplyRepository = repository;
			_jobRepository = jobRepository;
			_hubContext = hubContext;
			_userManager = userManager;
			_notificationService = notificationService;

		}

		//public async Task ApplyAsync(JobApplicationDto dto)
		//{
		//	var job = await _jobRepository.GetJobByIdAsync(dto.JobId);
		//	if (job == null) throw new Exception("Job not found");

		//	if (await _ApplyRepository.AlreadyAppliedAsync(dto.JobId, dto.InterpreterId))
		//		throw new Exception("Already applied");

		//	var application = new JobApplication
		//	{
		//		JobId = dto.JobId,
		//		InterpreterId = dto.InterpreterId,
		//		Message = dto.Message,
		//		Status = "0"
		//	};

		//	await _ApplyRepository.AddAsync(application);
		//	await _ApplyRepository.SaveChangesAsync();
		//}

		public async Task ApplyAsync(JobApplicationDto dto)
		{
			// 1. Kiểm tra Job hợp lệ
			var job = await _jobRepository.GetJobByIdAsync(dto.JobId)
					   ?? throw new ArgumentException("Job not found");

			// 2. Chặn apply trùng
			if (await _ApplyRepository.AlreadyAppliedAsync(dto.JobId, dto.InterpreterId))
				throw new InvalidOperationException("Already applied");

			// 3. Tạo bản ghi ứng tuyển
			var application = new JobApplication
			{
				JobId = dto.JobId,
				InterpreterId = dto.InterpreterId,
				Message = dto.Message,
				Status = "0",            // Pending
				CreatedAt = DateTime.UtcNow // nếu có trường
			};

			await _ApplyRepository.AddAsync(application);
			await _ApplyRepository.SaveChangesAsync();

			// 4. --- GỬI THÔNG BÁO REAL-TIME ---
			// (a) Lấy thông tin interpreter (để hiện tên)
			var interpreter = await _userManager.FindByIdAsync(dto.InterpreterId.ToString());

			// (b) Payload gửi tới client
			var payload = new
			{
				JobId = job.Id,
				JobTitle = job.JobTitle,
				InterpreterId = interpreter.Id,
				InterpreterName = interpreter.FullName,
				AppliedAt = DateTime.UtcNow
			};

			await _notificationService.SendNotificationAsync(
	            job.CustomerId,
	           "Có người ứng tuyển",
	           $"Biên dịch viên {interpreter.FullName} đã ứng tuyển công việc {job.JobTitle}."
);


		}


		public async Task<List<JobApplication>> GetApplicationsForJobAsync(Guid jobId)
		{
			return await _ApplyRepository.GetByJobIdAsync(jobId);
		}

		public async Task SelectInterpreterAsync(SelectInterRequest selectInterRequest)
		{
			var job = await _jobRepository.GetJobByIdAsync(selectInterRequest.JobId);
			if (job == null)
				throw new Exception("Job not found");

			var application = await _ApplyRepository.GetByJobIdAsync(selectInterRequest.JobId)
				.ContinueWith(t => t.Result.FirstOrDefault(a => a.InterpreterId == selectInterRequest.InterpreterId));

			if (application == null)
				throw new Exception("Application not found");

			application.Status = "1"; // Accepted
			application.LastUpdatedAt = DateTimeOffset.UtcNow;

			// Update job's current hires count
			job.CurrentHires++;

			// Check if job is fully recruited
			if (job.CurrentHires >= job.RequiredHires)
			{
				job.Status = (int)JobStatus.FullyRecruited;
			}

			await _ApplyRepository.SaveChangesAsync();
		}



		public async Task<List<JobApplicationCardDto>> GetApplicationsByInterpreterId(Guid interpreterId)
		{
			var applications = await _ApplyRepository.GetByInterpreterIdAsync(interpreterId);

			return applications.Select(app => new JobApplicationCardDto
			{
				ApplicationId = app.Id,
				JobId = app.JobId,
				JobTitle = app.Job?.JobTitle ?? "Unknown",
				Price = app.Job?.HourlyRate != null ? $"${app.Job.HourlyRate / 1000}k" : "$0",
				Status = ConvertStatus(app.Status),
				DeadLine = app.Job?.Deadline,
				CreatedDate = app.CreatedAt
			}).ToList();
		}

		private string ConvertStatus(string status)
		{
			return status switch
			{
				"0" => "Pending",
				"1" => "Accepted",
				"2" => "Rejected",
				_ => "Unknown"
			};
		}

	}

}
