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
		Task<TranslatorCertificateDto?> GetByUserIdAsync(Guid userId);
		Task AddOrUpdateAsync(Guid userId, TranslatorCertificateCreateUpdateDto dto);
		Task DeleteAsync(Guid userId);
	}

}
