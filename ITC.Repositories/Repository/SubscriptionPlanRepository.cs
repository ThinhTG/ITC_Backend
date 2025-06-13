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
	public class SubscriptionPlanRepository : ISubscriptionPlanRepository
	{
		private readonly ITCDbContext _context;

		public SubscriptionPlanRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task<SubscriptionPlan?> GetByIdAsync(Guid id)
		{
			return await _context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Id == id);
		}

		public async Task<List<SubscriptionPlan>> GetAllAsync()
		{
			return await _context.SubscriptionPlans.ToListAsync();
		}
	}

}
