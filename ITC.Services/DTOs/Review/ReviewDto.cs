using ITC.Core.Utils;

public class ReviewDto
{
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public Guid JobId { get; set; }
    public int Rating { get; set; }
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; } =  TimeHelper.GetVietnameseTime();
} 