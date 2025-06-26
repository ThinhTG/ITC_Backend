using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ITC.Services.DTOs.Complaint;

namespace ITC.Services.Complaint
{
    public interface IComplaintService
    {
        Task<ComplaintDto> CreateComplaintAsync(Guid userId, ComplaintCreateDto dto);
        Task<IEnumerable<ComplaintDto>> GetComplaintsByUserAsync(Guid userId);
        Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync();
        Task ChangeStatusAsync(Guid complaintId, int status);
        Task<IEnumerable<ComplaintMessageDto>> GetMessagesAsync(Guid complaintId);
        Task<ComplaintMessageDto> SendMessageAsync(Guid complaintId, Guid senderId, ComplaintMessageCreateDto dto);
        Task ResolveComplaintAsync(Guid complaintId, ComplaintResolutionDto dto);
    }
} 