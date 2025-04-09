using ITC.BusinessObject.Entities;
using ITC.Services.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobService
{
	public interface IJobService
	{
		Task<IEnumerable<Job>> GetAllJobsAsync();
		Task<Job?> GetJobByIdAsync(Guid id);
		Task<Job> CreateJobAsync(Job job);
		Task<Job> UpdateJobAsync(Job job);
		Task<bool> DeleteJobAsync(Guid id);

		Task<IEnumerable<Job>> GetJobsFilteredAsync(JobFilterDto filter);
	}
}
