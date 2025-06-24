public class RevenueDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<RevenueOverTimeDto> RevenueOverTime { get; set; }
    public List<RevenueByCategoryDto> RevenueByCategory { get; set; }
    public List<RecentTransactionDto> RecentTransactions { get; set; }
}

public class RevenueOverTimeDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}

public class RevenueByCategoryDto
{
    public string Category { get; set; }
    public decimal Amount { get; set; }
    public double Percent { get; set; }
}

public class RecentTransactionDto
{
    public string Customer { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Category { get; set; }
    public string Source { get; set; }
} 