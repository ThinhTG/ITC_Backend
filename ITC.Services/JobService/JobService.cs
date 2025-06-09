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
					CreatedAt = DateTime.UtcNow
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
	}

	}


