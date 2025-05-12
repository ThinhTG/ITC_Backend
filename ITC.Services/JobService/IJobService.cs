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
		Task<bool> PostJobAsync(CreateJobRequest jobDto, Guid customerId);
	}
}
