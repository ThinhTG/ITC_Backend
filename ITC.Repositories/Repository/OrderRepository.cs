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
	public class OrderRepository : IOrderRepository
	{
		private readonly ITCDbContext _context;

		public OrderRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task AddAsync(Order order)
		{
			await _context.Orders.AddAsync(order);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Order order)
		{
			_context.Orders.Remove(order);
			await _context.SaveChangesAsync();
		}

		public async Task<IEnumerable<Order>> GetAllAsync()
		{
			return await _context.Orders
				.Include(o => o.Job)
				.Include(o => o.Customer)
				.Include(o => o.Interpreter)
				.ToListAsync();
		}

		public async Task<Order?> GetByIdAsync(int id)
		{
			return await _context.Orders
				.Include(o => o.Job)
				.Include(o => o.Customer)
				.Include(o => o.Interpreter)
				.FirstOrDefaultAsync(o => o.OrderId == id);
		}

		public async Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId)
		{
			return await _context.Orders
				.Where(o => o.CustomerId == customerId)
				.ToListAsync();
		}

		public async Task<IEnumerable<Order>> GetByInterpreterIdAsync(Guid interpreterId)
		{
			return await _context.Orders
				.Where(o => o.InterpreterId == interpreterId)
				.ToListAsync();
		}

		public async Task UpdateAsync(Order order)
		{
			_context.Orders.Update(order);
			await _context.SaveChangesAsync();
		}
	}
}
