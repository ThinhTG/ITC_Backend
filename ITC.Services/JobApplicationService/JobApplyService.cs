using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.JobApply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobApplicationService
{
	public class JobApplyService : IJobApplyService
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;

		public JobApplyService(IUnitOfWork unitOfWork, IMapper mapper)
		{
			_unitOfWork = unitOfWork;
			_mapper = mapper;
		}

		public async Task<bool> ApplyToJobAsync(Guid interpreterId, JobApplicationCreateDto dto)
		{
			var jobRepo = _unitOfWork.GetRepository<Job>();
			var jobApplicationRepo = _unitOfWork.GetRepository<JobApplication>();

			var job = await jobRepo.GetByIdAsync(dto.JobId);
			if (job == null)
				throw new Exception("Job not found.");

			var existingApplication = await jobApplicationRepo
				.FirstOrDefaultAsync(x => x.JobId == dto.JobId && x.InterpreterId == interpreterId);

			if (existingApplication != null)
				throw new Exception("You have already applied to this job.");

			var application = new JobApplication
			{
				Id = Guid.NewGuid(),
				JobId = dto.JobId,
				InterpreterId = interpreterId,
				Message = dto.Message,
				CreatedAt = DateTime.UtcNow,
				Status = 0
			};

			await jobApplicationRepo.InsertAsync(application);
			await _unitOfWork.SaveAsync();

			return true;
		}

		public Task<IEnumerable<JobApplicationDto>> GetApplicationsByInterpreterIdAsync(Guid interpreterId, JobApplicationFilterDto filter)
		{
			throw new NotImplementedException();
		}

		public Task<IEnumerable<JobApplicationDto>> GetApplicationsByJobIdAsync(Guid jobId)
		{
			throw new NotImplementedException();
		}
	} 
}
