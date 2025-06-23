public class Review
{
    public Guid Id { get; set; }
    public Guid ReviewerId { get; set; }
    public Guid RevieweeId { get; set; }
    public Guid JobId { get; set; }
    public int Rating { get; set; } // 1-5
    public string Comment { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
} 