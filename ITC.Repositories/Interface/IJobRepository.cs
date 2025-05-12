using ITC.BusinessObject.Entities;
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
		Task SaveChangesAsync();
	}
}
