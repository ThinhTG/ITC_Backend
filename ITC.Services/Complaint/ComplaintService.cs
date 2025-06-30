using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.Core.Enum;
using ITC.Core.Hubs;
using ITC.Core.Utils;
using ITC.Repositories.Interface;
using ITC.Services.DTOs.Complaint;
using ITC.Services.WalletService;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace ITC.Services.Complaint
{
    public class ComplaintService : IComplaintService
    {
        private readonly IComplaintRepository _complaintRepo;
        private readonly IComplaintMessageRepository _messageRepo;
        private readonly IWalletService _walletService;
        private readonly IJobRepository _jobRepository;
        private readonly IJobApplicationRepository _jobApplicationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ComplaintService(IComplaintRepository complaintRepo, IComplaintMessageRepository messageRepo, IWalletService walletService, IJobRepository jobRepository, IJobApplicationRepository jobApplicationRepository, UserManager<ApplicationUser> userManager, IHubContext<NotificationHub> hubContext)
        {
            _complaintRepo = complaintRepo;
            _messageRepo = messageRepo;
            _walletService = walletService;
            _jobRepository = jobRepository;
            _jobApplicationRepository = jobApplicationRepository;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        public async Task<ComplaintDto> CreateComplaintAsync(Guid userId, ComplaintCreateDto dto)
        {
            var jobApplication = await _jobApplicationRepository.GetByIdAsync(dto.RelatedJobApplicationId.Value);

            if (dto.RelatedJobApplicationId.HasValue)
            {
                if (jobApplication == null)
                {
                    throw new Exception($"JobApplication with id {dto.RelatedJobApplicationId} not found");
                }
            }

            if (jobApplication.InterpreterId != null)
            {
                var relatedUser = await _userManager.FindByIdAsync(jobApplication.InterpreterId.ToString());
                if (relatedUser == null)
                {
                    throw new Exception($"RelatedUser not found");
                }
            }

            var complaint = new BusinessObject.Entities.Complaint
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                ComplaintType = dto.ComplaintType,
                RelatedJobApplicationId = dto.RelatedJobApplicationId,
                RelatedUserId = jobApplication.InterpreterId,
                Status = ComplaintStatus.Processing,
                CreatedAt = TimeHelper.GetVietnameseTime(),
                UpdatedAt = TimeHelper.GetVietnameseTime(),
                ResolutionNotes = string.Empty
            };
            await _complaintRepo.AddAsync(complaint);

            // Tạo message đầu tiên
            if (!string.IsNullOrEmpty(dto.Message))
            {
                var message = new ComplaintMessage
                {
                    Id = Guid.NewGuid(),
                    ComplaintId = complaint.Id,
                    SenderId = userId,
                    Message = dto.Message,
                    Attachment = dto.Attachment ?? string.Empty,
                    SentAt = TimeHelper.GetVietnameseTime()
                };
                await _messageRepo.AddAsync(message);

                var messageDto = ToMessageDto(message);
                // Notify related user
                if (complaint.RelatedUserId.HasValue)
                {
                    await _hubContext.Clients.Group(complaint.RelatedUserId.Value.ToString()).SendAsync("ReceiveComplaintMessage", messageDto);
                }

                // Notify admins/staff
                var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                var adminAndStaffIds = staffUsers.Concat(adminUsers).Select(u => u.Id).ToList();

                foreach (var adminId in adminAndStaffIds)
                {
                    if (adminId != userId) // Don't send notification to self
                    {
                        await _hubContext.Clients.Group(adminId.ToString()).SendAsync("ReceiveComplaintMessage", messageDto);
                    }
                }
            }

            return ToComplaintDto(complaint);
        }

        public async Task<IEnumerable<ComplaintDto>> GetComplaintsByUserAsync(Guid userId)
        {
            var list = await _complaintRepo.GetByUserIdAsync(userId);
            return list.Select(ToComplaintDto);
        }

        public async Task<IEnumerable<ComplaintDto>> GetAllComplaintsAsync()
        {
            var list = await _complaintRepo.GetAllAsync();
            return list.Select(ToComplaintDto);
        }

        public async Task ChangeStatusAsync(Guid complaintId, int status)
        {
            var complaint = await _complaintRepo.GetByIdAsync(complaintId);
            if (complaint == null) throw new Exception("Complaint not found");
            complaint.Status = (ComplaintStatus)status;
            complaint.UpdatedAt = TimeHelper.GetVietnameseTime();
            await _complaintRepo.UpdateAsync(complaint);
        }

        public async Task<IEnumerable<ComplaintMessageDto>> GetMessagesAsync(Guid complaintId)
        {
            var list = await _messageRepo.GetByComplaintIdAsync(complaintId);
            return list.Select(ToMessageDto);
        }

        public async Task<ComplaintMessageDto> SendMessageAsync(Guid complaintId, Guid senderId, ComplaintMessageCreateDto dto)
        {
            var message = new ComplaintMessage
            {
                Id = Guid.NewGuid(),
                ComplaintId = complaintId,
                SenderId = senderId,
                Message = dto.Message,
                Attachment = dto.Attachment ?? string.Empty,
                SentAt = TimeHelper.GetVietnameseTime()
            };
            await _messageRepo.AddAsync(message);

            var complaint = await _complaintRepo.GetByIdAsync(complaintId);
            if (complaint == null) throw new Exception("Complaint not found");

            var messageDto = ToMessageDto(message);

            var recipients = new List<Guid>();
            if (complaint.UserId != senderId)
            {
                recipients.Add(complaint.UserId);
            }
            if (complaint.RelatedUserId.HasValue && complaint.RelatedUserId.Value != senderId)
            {
                recipients.Add(complaint.RelatedUserId.Value);
            }

            if (complaint.AdminId.HasValue && complaint.AdminId.Value != senderId)
            {
                recipients.Add(complaint.AdminId.Value);
            }
            else if (!complaint.AdminId.HasValue)
            {
                // Notify all staff and admins if no specific admin is assigned
                var staffUsers = await _userManager.GetUsersInRoleAsync("Staff");
                var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
                var allAdminStaff = staffUsers.Concat(adminUsers).ToList();
                foreach (var user in allAdminStaff)
                {
                    if (user.Id != senderId)
                    {
                        recipients.Add(user.Id);
                    }
                }
            }


            foreach (var recipientId in recipients.Distinct())
            {
                await _hubContext.Clients.Group(recipientId.ToString()).SendAsync("ReceiveComplaintMessage", messageDto);
            }

            return ToMessageDto(message);
        }

        public async Task ResolveComplaintAsync(Guid complaintId, ComplaintResolutionDto dto)
        {
            var complaint = await _complaintRepo.GetByIdAsync(complaintId);
            if (complaint == null) throw new Exception("Complaint not found");
            complaint.AmountToPayTalent = dto.AmountToPayTalent;
            complaint.AmountToRefundCustomer = dto.AmountToRefundCustomer;
            complaint.ResolutionNotes = dto.ResolutionNotes;
            complaint.Status = dto.Status;
            complaint.ResolvedAt = dto.ResolvedAt ?? TimeHelper.GetVietnameseTime();
            complaint.UpdatedAt = TimeHelper.GetVietnameseTime();

            if (complaint.RelatedJobApplicationId.HasValue)
            {
                var jobApplication = await _jobApplicationRepository.GetByIdAsync(complaint.RelatedJobApplicationId.Value);
                if (jobApplication != null)
                {
                    Guid? talentId = jobApplication.InterpreterId;
                    Guid? customerId = jobApplication.Job?.CustomerId;

                    // Chuyển tiền hoàn về ví Customer
                    if (dto.AmountToRefundCustomer > 0 && customerId.HasValue)
                    {
                        await _walletService.AddMoneyToWalletAsync(customerId.Value, dto.AmountToRefundCustomer, 0);
                    }
                    // Chuyển tiền cho Talent
                    if (dto.AmountToPayTalent > 0 && talentId.HasValue)
                    {
                        await _walletService.AddMoneyToWalletAsync(talentId.Value, dto.AmountToPayTalent, 0);
                    }
                }
            }

            await _complaintRepo.UpdateAsync(complaint);
        }

        // Mapping helpers
        private ComplaintDto ToComplaintDto(BusinessObject.Entities.Complaint c)
        {
            return new ComplaintDto
            {
                Id = c.Id,
                UserId = c.UserId,
                AdminId = c.AdminId,
                ComplaintType = c.ComplaintType,
                RelatedJobApplicationId = c.RelatedJobApplicationId,
                RelatedUserId = c.RelatedUserId,
                Status = c.Status,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt,
                AmountToPayTalent = c.AmountToPayTalent,
                AmountToRefundCustomer = c.AmountToRefundCustomer,
                ResolutionNotes = c.ResolutionNotes,
                ResolvedAt = c.ResolvedAt
            };
        }

        private ComplaintMessageDto ToMessageDto(ComplaintMessage m)
        {
            return new ComplaintMessageDto
            {
                Id = m.Id,
                ComplaintId = m.ComplaintId,
                SenderId = m.SenderId,
                Message = m.Message,
                Attachment = m.Attachment,
                SentAt = m.SentAt
            };
        }
    }
} 