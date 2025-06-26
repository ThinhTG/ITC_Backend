using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;

namespace ITC.Repositories.Interface
{
    public interface IComplaintMessageRepository
    {
        Task<ComplaintMessage> AddAsync(ComplaintMessage message);
        Task<IEnumerable<ComplaintMessage>> GetByComplaintIdAsync(Guid complaintId);
    }
} 