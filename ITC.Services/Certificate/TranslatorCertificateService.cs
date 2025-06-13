using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.Core.Contracts;
using ITC.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Certificate
{
	public class TranslatorCertificateService : ITranslatorCertificateService
	{
		private readonly ITranslatorCertificateRepository _repo;
		private readonly IMapper _mapper;

		public TranslatorCertificateService(
			ITranslatorCertificateRepository repo,
			IMapper mapper)
		{
			_repo = repo;
			_mapper = mapper;
		}

		public async Task<TranslatorCertificateDto?> GetByUserIdAsync(Guid userId)
		{
			var entity = await _repo.GetByUserIdAsync(userId);
			return entity == null ? null : _mapper.Map<TranslatorCertificateDto>(entity);
		}

		public async Task AddOrUpdateAsync(Guid userId, TranslatorCertificateCreateUpdateDto dto)
		{
			var existing = await _repo.GetByUserIdAsync(userId);
			if (existing == null)
			{
				var entity = _mapper.Map<TranslatorCertificate>(dto);
				entity.ApplicationUserId = userId;
				await _repo.AddAsync(entity);
			}
			else
			{
				_mapper.Map(dto, existing);
				existing.UpdatedAt = DateTime.UtcNow;
				await _repo.UpdateAsync(existing);
			}
		}

		public async Task DeleteAsync(Guid userId)
		{
			await _repo.DeleteAsync(userId);
		}
	}

}
