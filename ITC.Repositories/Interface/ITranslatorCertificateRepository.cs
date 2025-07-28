using ITC.BusinessObject.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Interface
{
	public interface ITranslatorCertificateRepository
	{
		Task<List<TranslatorCertificate>> GetByUserIdAsync(Guid userId);
		Task<TranslatorCertificate?> GetByIdAsync(Guid id);
		Task<TranslatorCertificate> AddAsync(TranslatorCertificate certificate);
		Task UpdateAsync(TranslatorCertificate certificate);
		Task DeleteAsync(Guid id);
		Task<List<TranslatorCertificate>> GetPendingCertificatesAsync();
		Task<List<TranslatorCertificate>> GetCertificatesByUserIdAsync(Guid userId);
	}
}
