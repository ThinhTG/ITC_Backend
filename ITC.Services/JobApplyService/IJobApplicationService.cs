using ITC.BusinessObject.Entities;
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
		Task SelectInterpreterAsync(Guid jobId, Guid InterpreterId);
	}
}
