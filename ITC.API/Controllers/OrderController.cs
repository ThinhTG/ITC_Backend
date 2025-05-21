using ITC.BusinessObject.Entities;
using ITC.Services.OrderService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
	public class OrderController : ControllerBase
	{
		private readonly IOrderService _orderService;

		public OrderController(IOrderService orderService)
		{
			_orderService = orderService;
		}

		[HttpGet]
		public async Task<IActionResult> GetAll()
		{
			var orders = await _orderService.GetAllAsync();
			return Ok(orders);
		}

		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(int id)
		{
			var order = await _orderService.GetByIdAsync(id);
			if (order == null) return NotFound();
			return Ok(order);
		}

		[HttpGet("customer/{customerId}")]
		public async Task<IActionResult> GetByCustomer(Guid customerId)
		{
			var orders = await _orderService.GetByCustomerAsync(customerId);
			return Ok(orders);
		}

		[HttpGet("interpreter/{interpreterId}")]
		public async Task<IActionResult> GetByInterpreter(Guid interpreterId)
		{
			var orders = await _orderService.GetByInterpreterAsync(interpreterId);
			return Ok(orders);
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Order order)
		{
			await _orderService.CreateAsync(order);
			return CreatedAtAction(nameof(GetById), new { id = order.OrderId }, order);
		}

		[HttpPut("{id}")]
		public async Task<IActionResult> Update(int id, [FromBody] Order updatedOrder)
		{
			if (id != updatedOrder.OrderId) return BadRequest("ID mismatch");

			var existing = await _orderService.GetByIdAsync(id);
			if (existing == null) return NotFound();

			await _orderService.UpdateAsync(updatedOrder);
			return NoContent();
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> Delete(int id)
		{
			var existing = await _orderService.GetByIdAsync(id);
			if (existing == null) return NotFound();

			await _orderService.DeleteAsync(id);
			return NoContent();
		}
	}
}
