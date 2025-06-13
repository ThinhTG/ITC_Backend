using AutoMapper;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.subplan;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.SubscriptionPlan
{
	public class SubscriptionPlanService : ISubscriptionPlanService
	{
		private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
		private readonly IMapper _mapper;

		public SubscriptionPlanService(ISubscriptionPlanRepository subscriptionPlanRepository, IMapper mapper)
		{
			_subscriptionPlanRepository = subscriptionPlanRepository;
			_mapper = mapper;
		}

		public async Task<IEnumerable<SubscriptionPlanDto>> GetAllPlansAsync()
		{
			var plans = await _subscriptionPlanRepository.GetAllAsync();
			return _mapper.Map<IEnumerable<SubscriptionPlanDto>>(plans);
		}

		public async Task<SubscriptionPlanDto?> GetPlanByIdAsync(Guid id)
		{
			var plan = await _subscriptionPlanRepository.GetByIdAsync(id);
			return plan == null ? null : _mapper.Map<SubscriptionPlanDto>(plan);
		}



	}

}
