using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using ITC.Repositories.Interface;

namespace ITC.Repositories.Repository
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly ITCDbContext _context;
        public ComplaintRepository(ITCDbContext context)
        {
            _context = context;
        }

        public async Task<Complaint> AddAsync(Complaint complaint)
        {
            _context.Complaints.Add(complaint);
            await _context.SaveChangesAsync();
            return complaint;
        }

        public async Task<Complaint> GetByIdAsync(Guid id)
        {
            return await _context.Complaints.FindAsync(id);
        }

        public async Task<IEnumerable<Complaint>> GetByUserIdAsync(Guid userId)
        {
            return _context.Complaints.Where(c => c.UserId == userId).ToList();
        }

        public async Task<IEnumerable<Complaint>> GetAllAsync()
        {
            return _context.Complaints.ToList();
        }

        public async Task UpdateAsync(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
        }
    }
} 