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
	public class JobApplicationRepository : IJobApplicationRepository
	{
		private readonly ITCDbContext _context;

		public JobApplicationRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task<bool> AlreadyAppliedAsync(Guid jobId, Guid interpreterId)
		{
			return await _context.JobApplications
				.AnyAsync(a => a.JobId == jobId && a.InterpreterId == interpreterId);
		}

		public async Task AddAsync(JobApplication application)
		{
			await _context.JobApplications.AddAsync(application);
		}

		public async Task<List<JobApplication>> GetByJobIdAsync(Guid jobId)
		{
			return await _context.JobApplications
				.Include(a => a.Interpreter)
				.Where(a => a.JobId == jobId)
				.ToListAsync();
		}

		public async Task SaveChangesAsync()
		{
			await _context.SaveChangesAsync();
		}

		public async Task<List<JobApplication>> GetByInterpreterIdAsync(Guid interpreterId)
		{
			return await _context.JobApplications
				.Include(app => app.Job)
				.Where(app => app.InterpreterId == interpreterId)
				.OrderByDescending(app => app.CreatedAt)
				.ToListAsync();
		}

		public IQueryable<JobApplication> GetJobApplicationsByInterpreterIdQueryable(Guid interpreterId)
		{
			return _context.JobApplications
				.Where(app => app.InterpreterId == interpreterId)
				.AsQueryable();
		}

		public async Task<JobApplication> GetByIdAsync(Guid id)
		{
			return await _context.JobApplications
				.Include(ja => ja.Job)
				.FirstOrDefaultAsync(ja => ja.Id == id);
		}

		public async Task Apply(JobApplication jobApplication)
		{
			await _context.JobApplications.AddAsync(jobApplication);
			await _context.SaveChangesAsync();
		}

	}

}
