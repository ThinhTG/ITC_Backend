using ITC.BusinessObject.Entities;

namespace ITC.Repositories.Interface
{
	public interface IJobApplicationRepository
    {
		Task<bool> AlreadyAppliedAsync(Guid jobId, Guid interpreterId);
		Task AddAsync(JobApplication application);
		Task<List<JobApplication>> GetByJobIdAsync(Guid jobId);
		Task SaveChangesAsync();

		Task<List<JobApplication>> GetByInterpreterIdAsync(Guid interpreterId);

		IQueryable<JobApplication> GetJobApplicationsByInterpreterIdQueryable(Guid interpreterId);

		Task<JobApplication> GetByIdAsync(Guid id);
		Task Apply(JobApplication jobApplication);

		Task<JobApplication> GetByJobAndInterpreterAsync(Guid jobId, Guid interpreterId);

		Task UpdateAsync(JobApplication jobApplication);
	}
}
