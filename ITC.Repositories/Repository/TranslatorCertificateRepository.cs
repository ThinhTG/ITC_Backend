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

		public async Task<List<TranslatorCertificate>> GetByUserIdAsync(Guid userId)
		{
			return await _context.TranslatorCertificates
				.Where(t => t.ApplicationUserId == userId)
				.ToListAsync();
		}

		public async Task<TranslatorCertificate?> GetByIdAsync(Guid id)
		{
			return await _context.TranslatorCertificates
				.FirstOrDefaultAsync(t => t.Id == id);
		}

		public async Task<TranslatorCertificate> AddAsync(TranslatorCertificate certificate)
		{
			certificate.Id = Guid.NewGuid();
			await _context.TranslatorCertificates.AddAsync(certificate);
			await _context.SaveChangesAsync();
			return certificate;
		}

		public async Task UpdateAsync(TranslatorCertificate certificate)
		{
			_context.TranslatorCertificates.Update(certificate);
			await _context.SaveChangesAsync();
		}

		public async Task DeleteAsync(Guid id)
		{
			var existing = await GetByIdAsync(id);
			if (existing != null)
			{
				_context.TranslatorCertificates.Remove(existing);
				await _context.SaveChangesAsync();
			}
		}
	}
}
