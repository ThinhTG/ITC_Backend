public class ReviewSummaryDto
{
    public double AverageRating { get; set; }
    public int TotalReviews { get; set; }
    public int[] StarCounts { get; set; } = new int[5]; // 0: 1 sao, 4: 5 sao
} 