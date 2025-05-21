using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
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

		public async Task<List<Job>> GetAllAsync()
		{
			return await _context.Set<Job>()
				.AsNoTracking()
				.ToListAsync();
		}

		public async Task<Job> GetJobByIdAsync(Guid jobId)
		{
			return await _context.Set<Job>()
				.AsNoTracking()
				.FirstOrDefaultAsync(j => j.Id == jobId);
		}

		public async Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId)
		{
			return await _context.Set<Job>()
								 .Where(j => j.CustomerId == customerId)
								 .OrderByDescending(j => j.CreatedAt)
								 .ToListAsync();
		}


		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
