using ITC.BusinessObject.Entities;
using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.OrderService
{ 
		public class OrderService : IOrderService
		{
			private readonly IOrderRepository _orderRepository;

			public OrderService(IOrderRepository orderRepository)
			{
				_orderRepository = orderRepository;
			}

			public async Task<IEnumerable<Order>> GetAllAsync()
			{
				return await _orderRepository.GetAllAsync();
			}

			public async Task<Order?> GetByIdAsync(int id)
			{
				return await _orderRepository.GetByIdAsync(id);
			}

			public async Task<IEnumerable<Order>> GetByCustomerAsync(Guid customerId)
			{
				return await _orderRepository.GetByCustomerIdAsync(customerId);
			}

			public async Task<IEnumerable<Order>> GetByInterpreterAsync(Guid interpreterId)
			{
				return await _orderRepository.GetByInterpreterIdAsync(interpreterId);
			}

			public async Task CreateAsync(Order order)
			{
				// Tính phí và tổng tiền
				order.PlatformFee = Math.Round(order.ServicePrice * 0.10M, 2);
				order.TotalPrice = order.ServicePrice + order.PlatformFee;
				order.Status = "Pending";
				order.CreatedAt = DateTime.UtcNow;

				await _orderRepository.AddAsync(order);
			}

			public async Task UpdateAsync(Order order)
			{
				order.UpdatedAt = DateTime.UtcNow;
				await _orderRepository.UpdateAsync(order);
			}

			public async Task DeleteAsync(int id)
			{
				var existing = await _orderRepository.GetByIdAsync(id);
				if (existing != null)
				{
					await _orderRepository.DeleteAsync(existing);
				}
			}
		}
}
