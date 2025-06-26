using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;

namespace ITC.Repositories.Interface
{
    public interface IComplaintRepository
    {
        Task<Complaint> AddAsync(Complaint complaint);
        Task<Complaint> GetByIdAsync(Guid id);
        Task<IEnumerable<Complaint>> GetByUserIdAsync(Guid userId);
        Task<IEnumerable<Complaint>> GetAllAsync();
        Task UpdateAsync(Complaint complaint);
    }
} 