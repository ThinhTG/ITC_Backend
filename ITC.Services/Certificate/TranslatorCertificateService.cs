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

		public async Task<List<TranslatorCertificateDto>> GetByUserIdAsync(Guid userId)
		{
			var entities = await _repo.GetByUserIdAsync(userId);
			return _mapper.Map<List<TranslatorCertificateDto>>(entities);
		}

		public async Task<TranslatorCertificateDto> GetByIdAsync(Guid id)
		{
			var entity = await _repo.GetByIdAsync(id);
			if (entity == null)
				throw new KeyNotFoundException($"Certificate with ID {id} not found.");
			return _mapper.Map<TranslatorCertificateDto>(entity);
		}

		public async Task<TranslatorCertificateDto> AddAsync(Guid userId, TranslatorCertificateCreateUpdateDto dto)
		{
			var entity = _mapper.Map<TranslatorCertificate>(dto);
			entity.ApplicationUserId = userId;
			var result = await _repo.AddAsync(entity);
			return _mapper.Map<TranslatorCertificateDto>(result);
		}

		public async Task UpdateAsync(Guid id, TranslatorCertificateCreateUpdateDto dto)
		{
			var existing = await _repo.GetByIdAsync(id);
			if (existing == null)
				throw new KeyNotFoundException($"Certificate with ID {id} not found.");

			_mapper.Map(dto, existing);
			existing.UpdatedAt = DateTimeOffset.UtcNow;
			await _repo.UpdateAsync(existing);
		}

		public async Task DeleteAsync(Guid id)
		{
			await _repo.DeleteAsync(id);
		}
	}
}
