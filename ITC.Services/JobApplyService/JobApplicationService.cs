using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.JobApply;
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

		public JobApplicationService(IJobApplicationRepository repository, IJobRepository jobRepository)
		{
			_ApplyRepository = repository;
			_jobRepository = jobRepository;
		}

		public async Task ApplyAsync(JobApplicationDto dto)
		{
			var job = await _jobRepository.GetJobByIdAsync(dto.JobId);
			if (job == null) throw new Exception("Job not found");

			if (await _ApplyRepository.AlreadyAppliedAsync(dto.JobId, dto.InterpreterId))
				throw new Exception("Already applied");

			var application = new JobApplication
			{
				JobId = dto.JobId,
				InterpreterId = dto.InterpreterId,
				Message = dto.Message,
				Status = "0"
			};

			await _ApplyRepository.AddAsync(application);
			await _ApplyRepository.SaveChangesAsync();
		}

		public async Task<List<JobApplication>> GetApplicationsForJobAsync(Guid jobId)
		{
			return await _ApplyRepository.GetByJobIdAsync(jobId);
		}

		public async Task SelectInterpreterAsync(Guid jobId, Guid intreId)
		{
			var applications = await _ApplyRepository.GetByJobIdAsync(jobId);
			if (!applications.Any())
				throw new Exception("No applications found");

			foreach (var app in applications)
			{
				app.Status = app.InterpreterId == intreId ? "1" : "2";
				app.LastUpdatedAt = DateTime.UtcNow;
			}

			await _ApplyRepository.SaveChangesAsync();
		}
	}

}
