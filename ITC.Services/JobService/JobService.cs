using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Request;
using ITC.Core;
using ITC.Core.Base;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using ITC.Repositories.PaggingItems;
using ITC.Repositories.Repository;
using ITC.Services.DTOs;
using ITC.Services.DTOs.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using ITC.Core.Enum;

namespace ITC.Services.JobService
{
	public class JobService : IJobService
	{
		private readonly IJobRepository _jobRepo;
		private readonly UploadSettings _uploadSettings;
		private readonly IMapper _mapper;

		public JobService(IJobRepository jobRepo, IOptions<UploadSettings> uploadSettings, IMapper mapper)
		{
			_jobRepo = jobRepo;
			_uploadSettings = uploadSettings.Value;
			_mapper = mapper;
		}


		public async Task<Job?> GetJobDetailsByIdAsync(Guid jobId)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId);
			if (job == null) return null;

			return job;
		}

		public async Task<JobDetailsDto?> GetJobDetailsDtoByIdAsync(Guid jobId)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId);
			if (job == null) return null;

			var jobDetailsDto = new JobDetailsDto
			{
				Id = job.Id,
				CustomerId = job.CustomerId,
				JobTitle = job.JobTitle,
				TranslationType = job.TranslationType,
				SourceLanguage = job.SourceLanguage,
				TargetLanguage = job.TargetLanguage,
				Description = job.Description,
				UploadFileUrl = job.UploadFileUrl,
				WorkingTime = job.WorkingTime,
				WorkAddressLine = job.WorkAddressLine,
				WorkCity = job.WorkCity,
				WorkPostalCode = job.WorkPostalCode,
				WorkCountry = job.WorkCountry,
				Deadline = job.Deadline,
				ResultFileUrl = job.ResultFileUrl,
				CompletedAt = job.CompletedAt,
				CompletionOffsetMinutes = job.CompletionOffsetMinutes,
				HourlyRate = job.HourlyRate,
				PlatformServiceFee = job.PlatformServiceFee,
				TotalFee = job.TotalFee,
				CompanyName = job.CompanyName,
				CompanyDescription = job.CompanyDescription,
				CompanyLogoUrl = job.CompanyLogoUrl,
				ContactEmail = job.ContactEmail,
				ContactPhone = job.ContactPhone,
				ContactAddress = job.ContactAddress,
				Status = job.Status,
				RequiredHires = job.RequiredHires,
				CurrentHires = job.CurrentHires,
				CreatedAt = job.CreatedAt,
				CustomerName = job.Customer?.FullName,
				CustomerEmail = job.Customer?.Email,
				Applications = job.Applications?.Select(app => new JobApplicationSummaryDto
				{
					Id = app.Id,
					InterpreterId = app.InterpreterId,
					InterpreterName = app.Interpreter?.FullName ?? string.Empty,
					InterpreterEmail = app.Interpreter?.Email ?? string.Empty,
					Message = app.Message,
					CreatedAt = app.CreatedAt,
					LastUpdatedAt = app.LastUpdatedAt,
					ApplicationStatus = app.ApplicationStatus,
					WorkStatus = app.WorkStatus,
					IsPaid = app.IsPaid,
					IndividualFee = app.IndividualFee,
					PaidAt = app.PaidAt,
					IndividualResultFileUrl = app.IndividualResultFileUrl,
					StartedAt = app.StartedAt,
					CompletedAt = app.CompletedAt,
					CompletionOffsetMinutes = app.CompletionOffsetMinutes
				}).ToList() ?? new List<JobApplicationSummaryDto>(),
				TotalHiredInterpreters = job.TotalHiredInterpreters,
				TotalInProgressInterpreters = job.TotalInProgressInterpreters,
				TotalCompletedInterpreters = job.TotalCompletedInterpreters,
				IsFullyRecruited = job.IsFullyRecruited,
				HasAnyInProgress = job.HasAnyInProgress,
				IsAllCompleted = job.IsAllCompleted
			};

			return jobDetailsDto;
		}

		public async Task<Guid> CreateJobAsync(CreateJobPostDto dto)
		{
			var job = new Job
			{
				Id = Guid.NewGuid(),
				JobTitle = dto.JobTitle,
				TranslationType = dto.TranslationType,
				SourceLanguage = dto.SourceLanguage,
				TargetLanguage = dto.TargetLanguage,
				Description = dto.Description,
				UploadFileUrl = dto.UploadFileUrl,
				HourlyRate = dto.HourlyRate,
				PlatformServiceFee = dto.PlatformServiceFee,
				TotalFee = dto.TotalFee,
				CompanyName = dto.CompanyName,
				CompanyDescription = dto.CompanyDescription,
				CompanyLogoUrl = dto.CompanyLogoUrl,
				ContactEmail = dto.ContactEmail,
				ContactPhone = dto.ContactPhone,
				ContactAddress = dto.ContactAddress,
				WorkAddressLine = dto.WorkAddressLine,
				WorkCity = dto.WorkCity,
				WorkPostalCode = dto.WorkPostalCode,
				WorkCountry = dto.WorkCountry,
				CustomerId = dto.CustomerId,
				CreatedAt = DateTimeOffset.UtcNow,
				Deadline = dto.Deadline,
				RequiredHires = dto.RequiredHires,
				CurrentHires = 0,
				WorkingTime = dto.WorkingTime
			};

			await _jobRepo.AddAsync(job);
			await _jobRepo.SaveChangesAsync();

			return job.Id;
		}

		public async Task<PaginatedList<JobDTO>> GetAllJobsAsync(JobFilterRequest request)
		{
			return await _jobRepo.GetFilteredJobsAsync(request);
		}


		public async Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId)
		{
			return await _jobRepo.GetJobsByCustomerIdAsync(customerId);
		}

		public async Task<bool> UpdateJobStatusAsync(Guid jobId, int newStatus)
		{
			var job = await _jobRepo.GetJobByIdAsync(jobId);
			if (job == null)
				return false;

			job.Status = newStatus;

			await _jobRepo.SaveChangesAsync();
			return true;
		}

		public async Task<PaginatedList<JobDTO>> GetJobsByCustomerIdPaginatedAsync(Guid customerId, int pageNumber, int pageSize)
		{
			var query = _jobRepo.GetJobsByCustomerIdQueryable(customerId);
			var pagedJobs = await PaginatedList<Job>.CreateAsync(query, pageNumber, pageSize);
			var jobDtos = pagedJobs.Items.Select(job => _mapper.Map<JobDTO>(job)).ToList();
			return new PaginatedList<JobDTO>(jobDtos, pagedJobs.TotalCount, pageNumber, pageSize);
		}
	}
}


