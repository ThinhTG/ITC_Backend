using ITC.BusinessObject.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

public interface IReviewRepository
{
    Task<Review> AddReviewAsync(Review review);
    Task<List<Review>> GetReviewsByUserAsync(Guid userId);
    Task<List<Review>> GetReviewsByJobAsync(Guid jobId);
    Task<List<Review>> GetReviewsByRevieweeAsync(Guid revieweeId);
    Task<bool> HasUserReviewedJobAsync(Guid reviewerId, Guid revieweeId, Guid jobId);
    Task<(double avgRating, int totalReviews, int[] starCounts)> GetReviewSummaryForUserAsync(Guid revieweeId);
} 