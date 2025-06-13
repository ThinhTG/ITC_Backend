using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Repositories.Repository
{
	public class TranslatorCertificateRepository : ITranslatorCertificateRepository
	{
		private readonly ITCDbContext _context;

		public TranslatorCertificateRepository(ITCDbContext context)
		{
			_context = context;
		}

		public async Task<TranslatorCertificate?> GetByUserIdAsync(Guid userId)
		{
			return await _context.TranslatorCertificates
			.FirstOrDefaultAsync(t => t.ApplicationUserId == userId);
		}

		public async Task AddAsync(TranslatorCertificate certificate)
		{
			await _context.TranslatorCertificates.AddAsync(certificate);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(TranslatorCertificate certificate)
		{
			_context.TranslatorCertificates.Update(certificate);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid userId)
		{
			var existing = await GetByUserIdAsync(userId);
			if (existing != null)
			{
				_context.TranslatorCertificates.Remove(existing);
				await _context.SaveChangesAsync();
			}
		}
	}

}
