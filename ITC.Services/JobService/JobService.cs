using ITC.BusinessObject.Entities;
using ITC.Core;
using ITC.Core.Base;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
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

		public JobService(IJobRepository jobRepo, IOptions<UploadSettings> uploadSettings)
		{
			_jobRepo = jobRepo;
			_uploadSettings = uploadSettings.Value;
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

		public async Task<BasePaginatedList<JobDTO>> GetAllJobsAsync(string? search, int pageIndex, int pageSize)
		{
			return await _jobRepo.GetAllJobsAsync(search, pageIndex, pageSize);
		}


	}

	}


