using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
using ITC.Core;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using ITC.Repositories.PaggingItems;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace ITC.Repositories.Repository
{
	public class JobRepository : IJobRepository
	{
		private readonly ITCDbContext _context;
		private readonly IMapper _mapper;


		public JobRepository(ITCDbContext context, IMapper mapper)
		{
			_context = context;
			_mapper = mapper;	
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

		public async Task<Job?> GetJobByIdAsync(Guid jobId)
		{
			return await _context.Jobs
				.FirstOrDefaultAsync(j => j.Id == jobId);
		}

		public async Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId)
		{
			return await _context.Set<Job>()
								 .Where(j => j.CustomerId == customerId)
								 .OrderByDescending(j => j.CreatedAt)
								 .ToListAsync();
		}

		public async Task<BasePaginatedList<JobDTO>> GetAllJobsAsync(string? search, int pageIndex, int pageSize)
		{
			var query = _context.Jobs.AsQueryable();

			if (!string.IsNullOrWhiteSpace(search))
			{
				search = search.ToLower();
				query = query.Where(j =>
					j.JobTitle.ToLower().Contains(search) ||
					j.CompanyName.ToLower().Contains(search) ||
					j.TranslationType.ToLower().Contains(search));
			}

			return await query.ToPagedListAsync<Job, JobDTO>(_mapper, pageIndex, pageSize);
		}


		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
