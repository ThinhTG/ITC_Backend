using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.Core.Contracts;
using ITC.Core.Enum;
using ITC.Core.Utils;
using ITC.Repositories.Interface;
using Microsoft.AspNetCore.Identity;
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
		private readonly UserManager<ApplicationUser> _userManager;
		private readonly IMapper _mapper;

		public TranslatorCertificateService(
			ITranslatorCertificateRepository repo,
			IMapper mapper, UserManager<ApplicationUser> userManager)
		{
			_repo = repo;
			_mapper = mapper;
			_userManager = userManager;
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
			var user = await _userManager.FindByIdAsync(userId.ToString());
			user.ApprovalStatus = UserApprovalStatus.PendingApproval;
			await _userManager.UpdateAsync(user);
			return _mapper.Map<TranslatorCertificateDto>(result);
		}

		public async Task UpdateAsync(Guid id, TranslatorCertificateCreateUpdateDto dto)
		{
			var existing = await _repo.GetByIdAsync(id);
			if (existing == null)
				throw new KeyNotFoundException($"Certificate with ID {id} not found.");

			_mapper.Map(dto, existing);
			existing.UpdatedAt = TimeHelper.GetVietnameseTime();
			await _repo.UpdateAsync(existing);
		}

		public async Task DeleteAsync(Guid id)
		{
			await _repo.DeleteAsync(id);
		}

		public async Task<List<TranslatorCertificateDto>> GetCertificatesByUserIdAsync(Guid userId)
		{
			var entities = await _repo.GetByUserIdAsync(userId);
			return _mapper.Map<List<TranslatorCertificateDto>>(entities);
		}

		public async Task<List<TranslatorCertificateDto>> GetPendingCertificatesAsync()
		{
			var entities = await _repo.GetPendingCertificatesAsync();
			return _mapper.Map<List<TranslatorCertificateDto>>(entities);
		}

		public async Task<bool> ApproveCertificateAsync(Guid certificateId)
		{
			var certificate = await _repo.GetByIdAsync(certificateId);
			if (certificate == null) return false;

			certificate.Status = CertificateStatus.Approved;
			certificate.ApprovedAt = TimeHelper.GetVietnameseTime();
			// TODO: Set ApprovedBy to current admin ID

			await _repo.UpdateAsync(certificate);
			return true;
		}

		public async Task<bool> RejectCertificateAsync(Guid certificateId, string reason)
		{
			var certificate = await _repo.GetByIdAsync(certificateId);
			if (certificate == null) return false;

			certificate.Status = CertificateStatus.Rejected;
			certificate.RejectReason = reason;
			certificate.UpdatedAt = TimeHelper.GetVietnameseTime();

			await _repo.UpdateAsync(certificate);
			return true;
		}
	}
}