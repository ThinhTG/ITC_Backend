using ITC.Services.DTOs.JobApply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobApplicationService
{
	public interface IJobApplyService
	{
		Task<bool> ApplyToJobAsync(Guid interpreterId, JobApplicationCreateDto dto);
		Task<IEnumerable<JobApplicationDto>> GetApplicationsByInterpreterIdAsync(Guid interpreterId, JobApplicationFilterDto filter);
		Task<IEnumerable<JobApplicationDto>> GetApplicationsByJobIdAsync(Guid jobId);
	}
}
