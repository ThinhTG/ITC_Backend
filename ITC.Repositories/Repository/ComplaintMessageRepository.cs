using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;

namespace ITC.Repositories.Repository
{
    public class ComplaintMessageRepository : IComplaintMessageRepository
    {
        private readonly ITCDbContext _context;
        public ComplaintMessageRepository(ITCDbContext context)
        {
            _context = context;
        }

        public async Task<ComplaintMessage> AddAsync(ComplaintMessage message)
        {
            _context.ComplaintMessages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        public async Task<IEnumerable<ComplaintMessage>> GetByComplaintIdAsync(Guid complaintId)
        {
            return _context.ComplaintMessages.Where(m => m.ComplaintId == complaintId).OrderBy(m => m.SentAt).ToList();
        }
    }
} 