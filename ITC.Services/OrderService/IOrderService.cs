using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.OrderService
{
	public interface IOrderService
	{
		Task<IEnumerable<Order>> GetAllAsync();
		Task<Order?> GetByIdAsync(int id);
		Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId);
		Task<IEnumerable<Order>> GetByInterpreterAsync(Guid interpreterId);
		Task CreateAsync(Order order);
		Task UpdateAsync(Order order);
		Task DeleteAsync(int id);
	}
}
