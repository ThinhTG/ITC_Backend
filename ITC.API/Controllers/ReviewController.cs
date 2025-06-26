using Microsoft.AspNetCore.Mvc;
using ITC.Services.JobApplyService;
using ITC.BusinessObject.Identity;

[ApiController]
[Route("api/[controller]")]
public class ReviewController : ControllerBase
{
    private readonly IReviewService _reviewService;
    private readonly IJobApplicationService _jobApplicationService;
    public ReviewController(IReviewService reviewService, IJobApplicationService jobApplicationService)
    {
        _reviewService = reviewService;
        _jobApplicationService = jobApplicationService;
    }

    /// <summary>
    /// Tạo mới một review cho một job (chỉ cho phép khi job đã hoàn thành và chưa review trước đó).
    /// </summary>
    /// <param name="reviewDto">Thông tin review (reviewerId, revieweeId, jobId, rating, comment)</param>
    /// <returns>Review vừa tạo</returns>
    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] ReviewDto reviewDto)
    {
        var result = await _reviewService.AddReviewAsync(reviewDto);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách review mà một user đã viết (reviewer).
    /// </summary>
    /// <param name="userId">Id của người review</param>
    /// <returns>Danh sách review</returns>
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetReviewsByUser(Guid userId)
    {
        var result = await _reviewService.GetReviewsByUserAsync(userId);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách review cho một job cụ thể.
    /// </summary>
    /// <param name="jobId">Id của job</param>
    /// <returns>Danh sách review</returns>
    [HttpGet("job/{jobId}")]
    public async Task<IActionResult> GetReviewsByJob(Guid jobId)
    {
        var result = await _reviewService.GetReviewsByJobAsync(jobId);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách review mà một user nhận được (reviewee).
    /// </summary>
    /// <param name="revieweeId">Id của người được review</param>
    /// <returns>Danh sách review</returns>
    [HttpGet("reviewee/{revieweeId}")]
    public async Task<IActionResult> GetReviewsByReviewee(Guid revieweeId)
    {
        var result = await _reviewService.GetReviewsByRevieweeAsync(revieweeId);
        return Ok(result);
    }

    /// <summary>
    /// Lấy tổng hợp điểm trung bình, tổng số review, phân bố số sao của một user.
    /// </summary>
    /// <param name="revieweeId">Id của người được review</param>
    /// <returns>Thông tin tổng hợp review</returns>
    [HttpGet("summary/{revieweeId}")]
    public async Task<IActionResult> GetReviewSummary(Guid revieweeId)
    {
        var result = await _reviewService.GetReviewSummaryForUserAsync(revieweeId);
        return Ok(result);
    }

    /// <summary>
    /// Lấy danh sách BPDV đã hoàn thành job để customer review
    /// </summary>
    /// <param name="jobId">Id của job</param>
    /// <returns>Danh sách BPDV đã hoàn thành job</returns>
    [HttpGet("job/{jobId}/reviewable-bpdv")]
    public async Task<IActionResult> GetReviewableBPDVForJob(Guid jobId)
    {
        var result = await _jobApplicationService.GetReviewableBPDVForJobAsync(jobId);
        return Ok(result);
    }
} 