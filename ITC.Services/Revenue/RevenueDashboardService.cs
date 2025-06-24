using ITC.Repositories.Interface;

namespace ITC.Services.Revenue
{
	public class RevenueDashboardService : IRevenueDashboardService
    {
        private readonly IWalletTransactionRepository _transactionRepo;
        public RevenueDashboardService(IWalletTransactionRepository transactionRepo)
        {
            _transactionRepo = transactionRepo;
        }

        public async Task<RevenueDashboardDto> GetDashboardAsync(DateTime? from, DateTime? to)
        {
            var transactions = await _transactionRepo.GetAllAsync(); // This already includes Wallet
            var query = transactions.AsQueryable();

            if (from.HasValue)
            {
                query = query.Where(t => t.TransactionDate >= from);
            }
            if (to.HasValue)
            {
                query = query.Where(t => t.TransactionDate <= to);
            }

            var filtered = query.Where(t => t.TransactionStatus == "success").ToList();

            var totalRevenue = filtered.Sum(t => t.Amount);
            var monthlyRevenue = filtered
                .Where(t => t.TransactionDate.Month == DateTime.UtcNow.Month && t.TransactionDate.Year == DateTime.UtcNow.Year)
                .Sum(t => t.Amount);
            var totalTransactions = filtered.Count;
            var avgValue = totalTransactions > 0 ? totalRevenue / totalTransactions : 0;

            var revenueOverTime = filtered
                .GroupBy(t => t.TransactionDate.Date)
                .Select(g => new RevenueOverTimeDto { Date = g.Key, Amount = g.Sum(x => x.Amount) })
                .OrderBy(x => x.Date)
                .ToList();

            var revenueByCategory = filtered
                .GroupBy(t => t.TransactionType)
                .Select(g => new RevenueByCategoryDto
                {
                    Category = g.Key,
                    Amount = g.Sum(x => x.Amount),
                    Percent = totalRevenue > 0 ? Math.Round((double)g.Sum(x => x.Amount) * 100 / (double)totalRevenue, 1) : 0
                })
                .ToList();

            var recentTransactions = filtered
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .Select(t => new RecentTransactionDto
                {
                    Customer = "N/A", // Cần join với User để lấy tên
                    Date = t.TransactionDate.DateTime,
                    Amount = t.Amount,
                    Category = t.TransactionType,
                    Source = "Online" // Giả sử
                })
                .ToList();

            return new RevenueDashboardDto
            {
                TotalRevenue = totalRevenue,
                MonthlyRevenue = monthlyRevenue,
                TotalTransactions = totalTransactions,
                AverageTransactionValue = avgValue,
                RevenueOverTime = revenueOverTime,
                RevenueByCategory = revenueByCategory,
                RecentTransactions = recentTransactions
            };
        }
    }
} 