using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.JobWork
{
	public interface IJobWorkService
	{
		Task SubmitWorkAsync(Guid jobId, Guid interpreterId, string? resultFileUrl);
		Task ConfirmCompletionAsync(Guid jobId, Guid customerId);
	}

}
