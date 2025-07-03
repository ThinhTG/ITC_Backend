using ITC.Core.Base;

public interface IReviewService
{
    Task<BaseResponse<ReviewDto>> AddReviewAsync(ReviewDto reviewDto);
    Task<List<ReviewDto>> GetReviewsByUserAsync(Guid userId);
    Task<List<ReviewDto>> GetReviewsByJobAsync(Guid jobId);
    Task<List<ReviewDto>> GetReviewsByRevieweeAsync(Guid revieweeId);
    Task<bool> HasUserReviewedJobAsync(Guid reviewerId, Guid revieweeId, Guid jobId);
    Task<ReviewSummaryDto> GetReviewSummaryForUserAsync(Guid revieweeId);
} 