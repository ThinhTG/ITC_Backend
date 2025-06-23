using ITC.Core.Enum;
using ITC.Repositories.Interface;

public class ReviewService : IReviewService
{
    private readonly IReviewRepository _reviewRepository;
    private readonly IJobRepository _jobRepository;
    public ReviewService(IReviewRepository reviewRepository, IJobRepository jobRepository)
    {
        _reviewRepository = reviewRepository;
        _jobRepository = jobRepository;
    }

    public async Task<ReviewDto> AddReviewAsync(ReviewDto reviewDto)
    {
        // Kiểm tra đã review chưa
        bool hasReviewed = await _reviewRepository.HasUserReviewedJobAsync(reviewDto.ReviewerId, reviewDto.RevieweeId, reviewDto.JobId);
        if (hasReviewed)
            throw new Exception("Bạn đã review cho job này rồi!");
        // Kiểm tra trạng thái job
        var job = await _jobRepository.GetJobByIdAsync(reviewDto.JobId);
        if (job == null || job.Status != (int)JobStatus.Completed)
            throw new Exception("Chỉ có thể review khi job đã hoàn thành!");
        var review = new Review
        {
            ReviewerId = reviewDto.ReviewerId,
            RevieweeId = reviewDto.RevieweeId,
            JobId = reviewDto.JobId,
            Rating = reviewDto.Rating,
            Comment = reviewDto.Comment,
            CreatedAt = DateTimeOffset.UtcNow
        };
        var result = await _reviewRepository.AddReviewAsync(review);
        reviewDto.Id = result.Id;
        reviewDto.CreatedAt = result.CreatedAt;
        return reviewDto;
    }

    public async Task<List<ReviewDto>> GetReviewsByUserAsync(Guid userId)
    {
        var reviews = await _reviewRepository.GetReviewsByUserAsync(userId);
        return reviews.ConvertAll(r => new ReviewDto
        {
            Id = r.Id,
            ReviewerId = r.ReviewerId,
            RevieweeId = r.RevieweeId,
            JobId = r.JobId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<List<ReviewDto>> GetReviewsByJobAsync(Guid jobId)
    {
        var reviews = await _reviewRepository.GetReviewsByJobAsync(jobId);
        return reviews.ConvertAll(r => new ReviewDto
        {
            Id = r.Id,
            ReviewerId = r.ReviewerId,
            RevieweeId = r.RevieweeId,
            JobId = r.JobId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<List<ReviewDto>> GetReviewsByRevieweeAsync(Guid revieweeId)
    {
        var reviews = await _reviewRepository.GetReviewsByRevieweeAsync(revieweeId);
        return reviews.ConvertAll(r => new ReviewDto
        {
            Id = r.Id,
            ReviewerId = r.ReviewerId,
            RevieweeId = r.RevieweeId,
            JobId = r.JobId,
            Rating = r.Rating,
            Comment = r.Comment,
            CreatedAt = r.CreatedAt
        });
    }

    public async Task<bool> HasUserReviewedJobAsync(Guid reviewerId, Guid revieweeId, Guid jobId)
    {
        return await _reviewRepository.HasUserReviewedJobAsync(reviewerId, revieweeId, jobId);
    }

    public async Task<ReviewSummaryDto> GetReviewSummaryForUserAsync(Guid revieweeId)
    {
        var (avg, total, starCounts) = await _reviewRepository.GetReviewSummaryForUserAsync(revieweeId);
        return new ReviewSummaryDto
        {
            AverageRating = avg,
            TotalReviews = total,
            StarCounts = starCounts
        };
    }
} 