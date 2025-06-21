using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.Core.Enum;
using ITC.Repositories.Interface;
using ITC.Repositories.PaggingItems;
using ITC.Services.DTOs.Withdrawal;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ITC.Services.WithdrawalService
{
    public class WithdrawalRequestService : IWithdrawalRequestService
    {
        private readonly IWithdrawalRequestRepository _withdrawalRequestRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public WithdrawalRequestService(
            IWithdrawalRequestRepository withdrawalRequestRepository,
            IWalletRepository walletRepository,
            UserManager<ApplicationUser> userManager)
        {
            _withdrawalRequestRepository = withdrawalRequestRepository;
            _walletRepository = walletRepository;
            _userManager = userManager;
        }

        public async Task<WithdrawalRequestDto> CreateAsync(Guid accountId, CreateWithdrawalRequestDto dto)
        {
            // Check if user has sufficient balance
            var wallet = await _walletRepository.GetWalletByAccountIdAsync(accountId);
            if (wallet == null)
                throw new Exception("Wallet not found");

            if (wallet.Balance < dto.Amount)
                throw new Exception("Insufficient balance");

            // Get user's bank account information
            var user = await _userManager.FindByIdAsync(accountId.ToString());
            if (user == null)
                throw new Exception("User not found");

            if (string.IsNullOrEmpty(user.BankAccountNumber) || 
                string.IsNullOrEmpty(user.BankName) || 
                string.IsNullOrEmpty(user.BankAccountHolderName))
                throw new Exception("Please update your bank account information before making a withdrawal request");

            var request = new WithdrawalRequest
            {
                AccountId = accountId,
                Amount = dto.Amount,
                BankAccountNumber = user.BankAccountNumber,
                BankName = user.BankName,
                BankAccountHolderName = user.BankAccountHolderName,
                Note = dto.Note,
                Status = WithdrawalStatus.Pending,
                RequestDate = DateTime.UtcNow
            };

            var createdRequest = await _withdrawalRequestRepository.CreateAsync(request);
            return await MapToDto(createdRequest);
        }

        public async Task<WithdrawalRequestDto> GetByIdAsync(Guid id)
        {
            var request = await _withdrawalRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Withdrawal request not found");

            return await MapToDto(request);
        }

        public async Task<IEnumerable<WithdrawalRequestDto>> GetByAccountIdAsync(Guid accountId)
        {
            var requests = await _withdrawalRequestRepository.GetByAccountIdAsync(accountId);
            var dtos = new List<WithdrawalRequestDto>();

            foreach (var request in requests)
            {
                dtos.Add(await MapToDto(request));
            }

            return dtos;
        }

        public async Task<PaginatedList<WithdrawalRequestDto>> GetAllAsync(int pageNumber, int pageSize)
        {
            var requests = await _withdrawalRequestRepository.GetPaginatedAsync(pageNumber, pageSize);
            var dtos = new List<WithdrawalRequestDto>();

            foreach (var request in requests.Items)
            {
                dtos.Add(await MapToDto(request));
            }

            return new PaginatedList<WithdrawalRequestDto>(
                dtos,
                requests.TotalCount,
                requests.PageNumber,
                requests.PageSize
            );
        }

        public async Task<WithdrawalRequestDto> UpdateStatusAsync(Guid id, UpdateWithdrawalRequestDto dto, Guid staffId)
        {
            var request = await _withdrawalRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Withdrawal request not found");

            // Validate status transition
            if (!IsValidStatusTransition(request.Status, dto.Status))
                throw new Exception($"Invalid status transition from {request.Status} to {dto.Status}");

            request.Status = dto.Status;
            request.Note = dto.Note;
            request.ProcessedBy = staffId;
            request.ProcessedDate = DateTime.UtcNow;

            // If status is WaitingForConfirmation (staff confirmed transfer), deduct from wallet
            if (dto.Status == WithdrawalStatus.WaitingForConfirmation)
            {
                var wallet = await _walletRepository.GetWalletByAccountIdAsync(request.AccountId);
                if (wallet == null)
                    throw new Exception("Wallet not found");

                if (wallet.Balance < request.Amount)
                    throw new Exception("Insufficient balance");

                wallet.Balance -= request.Amount;
                await _walletRepository.UpdateWalletAsync(wallet);
            }

            await _withdrawalRequestRepository.UpdateAsync(request);
            return await MapToDto(request);
        }

        /// <summary>
        /// BPDV xác nhận đã nhận được tiền
        /// </summary>
        public async Task<WithdrawalRequestDto> ConfirmReceivedAsync(Guid id, Guid accountId)
        {
            var request = await _withdrawalRequestRepository.GetByIdAsync(id);
            if (request == null)
                throw new Exception("Withdrawal request not found");

            if (request.AccountId != accountId)
                throw new Exception("You can only confirm your own withdrawal requests");

            if (request.Status != WithdrawalStatus.WaitingForConfirmation)
                throw new Exception("Can only confirm requests that are waiting for confirmation");

            request.Status = WithdrawalStatus.Completed;
            request.ProcessedDate = DateTime.UtcNow;

            await _withdrawalRequestRepository.UpdateAsync(request);
            return await MapToDto(request);
        }

        public async Task<bool> CancelRequestAsync(Guid requestId, Guid accountId)
        {
            var request = await _withdrawalRequestRepository.GetByIdAsync(requestId);
            if (request == null) return false;
            if (request.AccountId != accountId) return false;
            if (request.Status != WithdrawalStatus.Pending) return false;

            request.Status = WithdrawalStatus.Canceled;
            request.ProcessedDate = DateTime.UtcNow;
            await _withdrawalRequestRepository.UpdateAsync(request);
            return true;
        }

        private bool IsValidStatusTransition(WithdrawalStatus currentStatus, WithdrawalStatus newStatus)
        {
            return (currentStatus, newStatus) switch
            {
                (WithdrawalStatus.Pending, WithdrawalStatus.WaitingForConfirmation) => true, // Staff confirms transfer
                (WithdrawalStatus.WaitingForConfirmation, WithdrawalStatus.Completed) => true, // BPDV confirms received
                _ => false
            };
        }

        private async Task<WithdrawalRequestDto> MapToDto(WithdrawalRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.AccountId.ToString());
            var processedByUser = request.ProcessedBy.HasValue 
                ? await _userManager.FindByIdAsync(request.ProcessedBy.Value.ToString())
                : null;

            return new WithdrawalRequestDto
            {
                WithdrawalRequestId = request.WithdrawalRequestId,
                AccountId = request.AccountId,
                UserName = user?.UserName,
                Email = user?.Email,
                PhoneNumber = user?.PhoneNumber,
                Amount = request.Amount,
                Status = request.Status,
                RequestDate = request.RequestDate,
                ProcessedDate = request.ProcessedDate,
                BankAccountNumber = request.BankAccountNumber,
                BankName = request.BankName,
                BankAccountHolderName = request.BankAccountHolderName,
                Note = request.Note,
                ProcessedBy = request.ProcessedBy,
                ProcessedByName = processedByUser?.UserName
            };
        }
    }
} 