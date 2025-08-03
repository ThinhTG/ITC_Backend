using AutoMapper;
using ITC.BusinessObject.DTOs.User;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Request;
using ITC.BusinessObject.Response;
using ITC.Core.Enum;
using ITC.Services.Certificate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITC.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly ITranslatorCertificateService _certificateService;
        private readonly IReviewService _reviewService;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<ApplicationRole> roleManager,
            IMapper mapper,
            ITranslatorCertificateService certificateService,
            IReviewService reviewService)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
            _certificateService = certificateService;
            _reviewService = reviewService;
        }

        public async Task<UserCountByRoleDto> GetUserCountByRoleAsync()
        {
            var customerRole = await _roleManager.FindByNameAsync("Customer");
            var talentRole = await _roleManager.FindByNameAsync("Talent");

            var customerCount = await _userManager.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == customerRole.Id))
                .CountAsync();

            var talentCount = await _userManager.Users
                .Where(u => u.UserRoles.Any(ur => ur.RoleId == talentRole.Id))
                .CountAsync();

            return new UserCountByRoleDto
            {
                CustomerCount = customerCount,
                TalentCount = talentCount
            };
        }

        public async Task<IEnumerable<UserResponse>> GetPendingApprovalUsersAsync()
        {
            var users = await _userManager.Users
                                          .Where(u => u.ApprovalStatus == UserApprovalStatus.PendingApproval)
                                          .ToListAsync();
            return _mapper.Map<IEnumerable<UserResponse>>(users);
        }

        public async Task<bool> ApproveUserAsync(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.ApprovalStatus = UserApprovalStatus.Approved;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<bool> RejectUserAsync(Guid userId, RejectUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return false;

            user.ApprovalStatus = UserApprovalStatus.Rejected;
            user.RejectReason = request.Reason;
            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        public async Task<UserResponse> GetUserDetailAsync(Guid userId)
		{
			var user = await _userManager.FindByIdAsync(userId.ToString());
			if (user == null) return null;

			return _mapper.Map<UserResponse>(user);
		}

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync()
		{
			var users = await _userManager.Users.ToListAsync();
			return _mapper.Map<IEnumerable<UserResponse>>(users);
		}

		public async Task<IEnumerable<UserResponse>> GetAllTalentUsersAsync()
		{
			// Lấy danh sách user trong role "Talent"
			var talentUsers = await _userManager.GetUsersInRoleAsync("Talent");

			return _mapper.Map<IEnumerable<UserResponse>>(talentUsers);
		}

		public async Task<IEnumerable<TalentWithCertificatesResponse>> GetAllTalentUsersWithCertificatesAsync()
		{
			// Lấy danh sách user trong role "Talent"
			var talentUsers = await _userManager.GetUsersInRoleAsync("Talent");
			var result = new List<TalentWithCertificatesResponse>();

			foreach (var user in talentUsers)
			{
				var talentResponse = _mapper.Map<TalentWithCertificatesResponse>(user);
				
				// Lấy certificates của user
				var certificates = await _certificateService.GetCertificatesByUserIdAsync(user.Id);
				talentResponse.Certificates = certificates;
				
				// Lấy thông tin rating của user
				var reviewSummary = await _reviewService.GetReviewSummaryForUserAsync(user.Id);
				talentResponse.AverageRating = reviewSummary.AverageRating;
				talentResponse.TotalReviews = reviewSummary.TotalReviews;
				talentResponse.StarCounts = reviewSummary.StarCounts;
				
				result.Add(talentResponse);
			}

			return result;
		}




		public async Task<UserApprovalStatusStatsDto> GetUserApprovalStatusStatsAsync()
        {
            var users = _userManager.Users;
            return new UserApprovalStatusStatsDto
            {
                NoCertificate = await users.CountAsync(u => u.ApprovalStatus == UserApprovalStatus.NoCertificate),
                PendingApproval = await users.CountAsync(u => u.ApprovalStatus == UserApprovalStatus.PendingApproval),
                Approved = await users.CountAsync(u => u.ApprovalStatus == UserApprovalStatus.Approved),
                Rejected = await users.CountAsync(u => u.ApprovalStatus == UserApprovalStatus.Rejected)
            };
        }
    }
} 