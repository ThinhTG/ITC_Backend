using ITC.BusinessObject.Entities;
using ITC.Repositories.Interface;
using ITC.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobService
{
	public class JobService : IJobService
	{
		private readonly IUnitOfWork _unitOfWork;

		public JobService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}

		public async Task<IEnumerable<Job>> GetAllJobsAsync()
		{
			var repo = _unitOfWork.GetRepository<Job>();
			return await repo.GetAllAsync();
		}

		public async Task<Job?> GetJobByIdAsync(Guid id)
		{
			var repo = _unitOfWork.GetRepository<Job>();
			return await repo.GetByIdAsync(id);
		}

		public async Task<Job> CreateJobAsync(Job job)
		{
			var repo = _unitOfWork.GetRepository<Job>();
			await repo.InsertAsync(job);
			await _unitOfWork.SaveAsync();
			return job;
		}

		public async Task<Job> UpdateJobAsync(Job job)
		{
			var repo = _unitOfWork.GetRepository<Job>();
			repo.Update(job);
			await _unitOfWork.SaveAsync();
			return job;
		}

		public async Task<bool> DeleteJobAsync(Guid id)
		{
			var repo = _unitOfWork.GetRepository<Job>();
			var job = await repo.GetByIdAsync(id);
			if (job == null)
				return false;

			repo.Delete(job.Id);
			await _unitOfWork.SaveAsync();
			return true;
		}

		public async Task<IEnumerable<Job>> GetJobsFilteredAsync(JobFilterDto filter)
		{
			var repo = _unitOfWork.GetRepository<Job>();
			var query = (await repo.GetAllAsync()).AsQueryable();

			if (!string.IsNullOrEmpty(filter.Location))
				query = query.Where(j => j.Location.Contains(filter.Location));

			if (filter.Status.HasValue)
				query = query.Where(j => j.Status == filter.Status.Value);

			if (filter.FromDate.HasValue)
				query = query.Where(j => j.JobDate >= filter.FromDate.Value);

			if (filter.ToDate.HasValue)
				query = query.Where(j => j.JobDate <= filter.ToDate.Value);

			return query
				.Skip((filter.Page - 1) * filter.PageSize)
				.Take(filter.PageSize)
				.ToList();
		}

	}
}
