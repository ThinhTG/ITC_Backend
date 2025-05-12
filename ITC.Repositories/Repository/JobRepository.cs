using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Repository
{
	public class JobRepository : IJobRepository
	{
		private readonly ITCDbContext _context;

		public JobRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Job job)
		{
			await _context.Jobs.AddAsync(job);
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
