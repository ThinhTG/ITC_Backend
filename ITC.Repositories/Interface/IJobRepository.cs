using ITC.BusinessObject.Entities;
using ITC.Core;
using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface IJobRepository
	{
		Task AddAsync(Job job);
		Task<List<Job>> GetAllAsync();

		Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId);

		Task<Job?> GetJobByIdAsync(Guid jobId);

		Task<BasePaginatedList<JobDTO>> GetAllJobsAsync(string? search, int pageIndex, int pageSize);
		Task SaveChangesAsync();
	}
}
