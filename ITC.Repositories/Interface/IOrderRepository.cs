using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
     public interface IOrderRepository
    {
		Task<Order?> GetByIdAsync(int id);
		Task<IEnumerable<Order>> GetAllAsync();
		Task<IEnumerable<Order>> GetByCustomerIdAsync(Guid customerId);
		Task<IEnumerable<Order>> GetByInterpreterIdAsync(Guid interpreterId);
		Task AddAsync(Order order);
		Task UpdateAsync(Order order);
		Task DeleteAsync(Order order);
	}
}
