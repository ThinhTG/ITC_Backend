public class RevenueDashboardDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public List<RevenueOverTimeDto> RevenueOverTime { get; set; }
    public List<RevenueByCategoryDto> RevenueByCategory { get; set; }
    public List<RecentTransactionDto> RecentTransactions { get; set; }
    public decimal TotalWithdrawals { get; set; }
    public decimal MonthlyWithdrawals { get; set; }
    public int TotalWithdrawalCount { get; set; }
    public decimal AverageWithdrawalValue { get; set; }
    public List<RecentWithdrawalDto> RecentWithdrawals { get; set; }
    public decimal TotalPlatformFees { get; set; }
    public decimal MonthlyPlatformFees { get; set; }
    public int TotalPlatformFeeCount { get; set; }
    public decimal AveragePlatformFeeValue { get; set; }
    public List<RecentPlatformFeeDto> RecentPlatformFees { get; set; }
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

public class RecentWithdrawalDto
{
    public string Customer { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string BankName { get; set; }
    public string Status { get; set; }
}

public class RecentPlatformFeeDto
{
    public string JobTitle { get; set; }
    public string Customer { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string JobType { get; set; }
} 