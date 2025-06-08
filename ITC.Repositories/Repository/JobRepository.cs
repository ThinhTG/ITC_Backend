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
using ITC.BusinessObject.Request;

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
				.Include(j => j.Customer)
				.Include(j => j.SelectedInterpreter) // Include selected interpreter if needed
				.FirstOrDefaultAsync(j => j.Id == jobId);
		}


		public async Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId)
		{
			return await _context.Set<Job>()
								 .Where(j => j.CustomerId == customerId)
								 .OrderByDescending(j => j.CreatedAt)
								 .ToListAsync();
		}

		public async Task<BasePaginatedList<JobDTO>> GetAllJobsAsync(JobFilterParams p)
		{
			var query = _context.Jobs.AsNoTracking().AsQueryable();

			/* ---------- Filter ---------- */
			if (!string.IsNullOrWhiteSpace(p.Search))
			{
				var kw = p.Search.ToLower();
				query = query.Where(j =>
					EF.Functions.Like(j.JobTitle.ToLower(), $"%{kw}%") ||
					EF.Functions.Like((j.CompanyName ?? string.Empty).ToLower(), $"%{kw}%") ||
					EF.Functions.Like(j.TranslationType.ToLower(), $"%{kw}%"));
			}

			if (!string.IsNullOrWhiteSpace(p.Location))
				query = query.Where(j => j.WorkAddressLine == p.Location);

			if (p.Categories?.Any() == true)
				query = query.Where(j => p.Categories.Contains(j.TranslationType));

			if (p.SourceLanguages?.Any() == true)
				query = query.Where(j => p.SourceLanguages.Contains(j.SourceLanguage));

			if (p.TargetLanguages?.Any() == true)
				query = query.Where(j => p.TargetLanguages.Contains(j.TargetLanguage));

			if (p.MinSalary.HasValue)
				query = query.Where(j => j.HourlyRate >= p.MinSalary.Value);

			if (p.MaxSalary.HasValue)
				query = query.Where(j => j.HourlyRate <= p.MaxSalary.Value);

			/* ---------- Paging + Mapping ---------- */
			return await query
				.OrderByDescending(j => j.CreatedAt)
				.ToPagedListAsync<Job, JobDTO>(_mapper, p.PageIndex, p.PageSize);
		}



		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
