using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
    public interface IJobApplicationRepository
    {
		Task<bool> AlreadyAppliedAsync(Guid jobId, Guid interpreterId);
		Task AddAsync(JobApplication application);
		Task<List<JobApplication>> GetByJobIdAsync(Guid jobId);
		Task SaveChangesAsync();
	}
}
