using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Request;
using ITC.Services.DTOs.JobApply;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobApplyService
{
	public interface IJobApplicationService
	{
		Task ApplyAsync(JobApplicationDto dto);
		Task<List<JobApplication>> GetApplicationsForJobAsync(Guid jobId);
		Task<List<JobApplicationViewDto>> GetApplicationsForJobWithDetailsAsync(Guid jobId);
		Task SelectInterpreterAsync(SelectInterRequest selectInterRequest);
		Task RejectInterpreterAsync(SelectInterRequest rejectRequest);

		Task<List<JobApplicationCardDto>> GetApplicationsByInterpreterId(Guid interpreterId);

		Task SaveChangesAsync();
	}
}