public class SubscriptionDashboardDto
{
    public int TotalSubscriptions { get; set; }
    public int ActiveSubscriptions { get; set; }
    public decimal SubscriptionRevenue { get; set; }
    public List<PlanDistributionDto> DistributionByPlan { get; set; }
    public List<SubscriptionTrendDto> TrendOverTime { get; set; }
    public List<ActiveSubscriptionDto> ActiveSubscriptionList { get; set; }
}

public class PlanDistributionDto
{
    public string Plan { get; set; }
    public int Count { get; set; }
    public double Percent { get; set; }
}

public class SubscriptionTrendDto
{
    public DateTime Date { get; set; }
    public int Count { get; set; }
}

public class ActiveSubscriptionDto
{
    public string Customer { get; set; }
    public string Plan { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal Amount { get; set; }
    public string Status { get; set; }
    public string Payment { get; set; }
} 