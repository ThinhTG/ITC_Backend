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
		Task<TranslatorCertificate?> GetByUserIdAsync(Guid userId);
		Task AddAsync(TranslatorCertificate certificate);
		Task UpdateAsync(TranslatorCertificate certificate);
		Task DeleteAsync(Guid userId);
	}

}
