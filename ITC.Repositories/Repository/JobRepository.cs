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

		public async Task<PaginatedList<JobDTO>> GetFilteredJobsAsync(JobFilterRequest request)
		{
			var query = _context.Jobs.AsQueryable();

			if (!string.IsNullOrWhiteSpace(request.JobTitle))
				query = query.Where(j => j.JobTitle.Contains(request.JobTitle));

			if (!string.IsNullOrWhiteSpace(request.Location))
				query = query.Where(j =>
					(j.WorkCity != null && j.WorkCity.Contains(request.Location)) ||
					(j.WorkCountry != null && j.WorkCountry.Contains(request.Location)) ||
					(j.WorkAddressLine != null && j.WorkAddressLine.Contains(request.Location))
				);

			if (request.Categories?.Any() == true)
				query = query.Where(j => request.Categories.Contains(j.TranslationType));

			if (request.SourceLanguages?.Any() == true)
				query = query.Where(j => request.SourceLanguages.Contains(j.SourceLanguage));

			if (request.TargetLanguages?.Any() == true)
				query = query.Where(j => request.TargetLanguages.Contains(j.TargetLanguage));

			if (request.MinSalary.HasValue)
				query = query.Where(j => j.HourlyRate >= request.MinSalary.Value);

			if (request.MaxSalary.HasValue)
				query = query.Where(j => j.HourlyRate <= request.MaxSalary.Value);

			query = query.OrderByDescending(j => j.CreatedAt);

			var PagingJobs = await PaginatedList<Job>.CreateAsync(query, request.PageIndex, request.PageSize);

			var jobDtos = PagingJobs.Items
				.Select(job => _mapper.Map<JobDTO>(job))
				.ToList();

				return new PaginatedList<JobDTO>
				(jobDtos, PagingJobs.TotalCount, request.PageIndex, request.PageSize);
		}


		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}
	}
}
