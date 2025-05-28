using ITC.BusinessObject.Entities;
using ITC.Core;
using ITC.Core.Contracts;
using ITC.Services.DTOs;
using ITC.Services.DTOs.Job;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobService
{
	public interface IJobService
	{
		Task<Guid> CreateJobAsync(CreateJobPostDto dto);

		//Task<List<JobResponseDto>> GetAllAvailableJobsAsync();

		Task<Job?> GetJobDetailsByIdAsync(Guid jobId);

		Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId);

		Task<BasePaginatedList<JobDTO>> GetAllJobsAsync(string? search, int pageIndex, int pageSize);

	}
}
