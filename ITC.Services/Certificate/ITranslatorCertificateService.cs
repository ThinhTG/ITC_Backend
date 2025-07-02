using ITC.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.Certificate
{
	public interface ITranslatorCertificateService
	{
		Task<List<TranslatorCertificateDto>> GetByUserIdAsync(Guid userId);
		Task<TranslatorCertificateDto> GetByIdAsync(Guid id);
		Task<TranslatorCertificateDto> AddAsync(Guid userId, TranslatorCertificateCreateUpdateDto dto);
		Task UpdateAsync(Guid id, TranslatorCertificateCreateUpdateDto dto);
		Task DeleteAsync(Guid id);
		Task<List<TranslatorCertificateDto>> GetCertificatesByUserIdAsync(Guid userId);
		Task<List<TranslatorCertificateDto>> GetPendingCertificatesAsync();
		Task<bool> ApproveCertificateAsync(Guid certificateId);
		Task<bool> RejectCertificateAsync(Guid certificateId, string reason);
		Task<List<object>> GetPendingCertificatesWithTalentAsync();
	}
}
