using ITC.BusinessObject.Entities;
using Microsoft.EntityFrameworkCore;
using ITC.Repositories.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System;

public class ReviewRepository : IReviewRepository
{
    private readonly ITCDbContext _context;
    public ReviewRepository(ITCDbContext context)
    {
        _context = context;
    }

    public async Task<Review> AddReviewAsync(Review review)
    {
        _context.Reviews.Add(review);
        await _context.SaveChangesAsync();
        return review;
    }

    public async Task<List<Review>> GetReviewsByUserAsync(Guid userId)
    {
        return await _context.Reviews.Where(r => r.ReviewerId == userId).ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByJobAsync(Guid jobId)
    {
        return await _context.Reviews.Where(r => r.JobId == jobId).ToListAsync();
    }

    public async Task<List<Review>> GetReviewsByRevieweeAsync(Guid revieweeId)
    {
        return await _context.Reviews.Where(r => r.RevieweeId == revieweeId).ToListAsync();
    }

    public async Task<bool> HasUserReviewedJobAsync(Guid reviewerId, Guid revieweeId, Guid jobId)
    {
        return await _context.Reviews.AnyAsync(r => r.ReviewerId == reviewerId && r.RevieweeId == revieweeId && r.JobId == jobId);
    }

    public async Task<(double avgRating, int totalReviews, int[] starCounts)> GetReviewSummaryForUserAsync(Guid revieweeId)
    {
        var reviews = await _context.Reviews.Where(r => r.RevieweeId == revieweeId).ToListAsync();
        int total = reviews.Count;
        double avg = total > 0 ? reviews.Average(r => r.Rating) : 0;
        int[] starCounts = new int[5];
        foreach (var r in reviews)
        {
            if (r.Rating >= 1 && r.Rating <= 5)
                starCounts[r.Rating - 1]++;
        }
        return (avg, total, starCounts);
    }
} 