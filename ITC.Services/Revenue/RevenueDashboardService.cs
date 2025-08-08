using ITC.Repositories.Interface;
using ITC.BusinessObject.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ITC.Services.Revenue
{
	public class RevenueDashboardService : IRevenueDashboardService
    {
        private readonly IWalletTransactionRepository _transactionRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWithdrawalRequestRepository _withdrawalRequestRepo;
        private readonly IJobRepository _jobRepo;
        
        public RevenueDashboardService(
            IWalletTransactionRepository transactionRepo, 
            IWalletRepository walletRepo,
            UserManager<ApplicationUser> userManager,
            IWithdrawalRequestRepository withdrawalRequestRepo,
            IJobRepository jobRepo)
        {
            _transactionRepo = transactionRepo;
            _walletRepo = walletRepo;
            _userManager = userManager;
            _withdrawalRequestRepo = withdrawalRequestRepo;
            _jobRepo = jobRepo;
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

            // Get recent transactions with customer names
            var recentTransactionsWithUsers = new List<RecentTransactionDto>();
            var recentTransactions = filtered
                .OrderByDescending(t => t.TransactionDate)
                .Take(10)
                .ToList();

            foreach (var transaction in recentTransactions)
            {
                // Get the wallet for this transaction
                var wallet = await _walletRepo.GetWalletByIdAsync(transaction.WalletId);
                string customerName = "Unknown";
                
                if (wallet != null)
                {
                    // Get the user for this wallet
                    var user = await _userManager.FindByIdAsync(wallet.AccountId.ToString());
                    customerName = user?.FullName ?? user?.UserName ?? "Unknown";
                }

                recentTransactionsWithUsers.Add(new RecentTransactionDto
                {
                    Customer = customerName,
                    Date = transaction.TransactionDate.DateTime,
                    Amount = transaction.Amount,
                    Category = transaction.TransactionType,
                    Source = "Online" // Giả sử
                });
            }

            // --- Withdrawal statistics ---
            var allWithdrawals = await _withdrawalRequestRepo.GetAllAsync();
            var withdrawalQuery = allWithdrawals.AsQueryable();
            if (from.HasValue)
            {
                withdrawalQuery = withdrawalQuery.Where(w => w.RequestDate >= from);
            }
            if (to.HasValue)
            {
                withdrawalQuery = withdrawalQuery.Where(w => w.RequestDate <= to);
            }
            var completedWithdrawals = withdrawalQuery.Where(w => w.Status == ITC.Core.Enum.WithdrawalStatus.Completed).ToList();
            var totalWithdrawals = completedWithdrawals.Sum(w => w.Amount);
            var monthlyWithdrawals = completedWithdrawals
                .Where(w => w.RequestDate.Month == DateTime.UtcNow.Month && w.RequestDate.Year == DateTime.UtcNow.Year)
                .Sum(w => w.Amount);
            var totalWithdrawalCount = completedWithdrawals.Count;
            var avgWithdrawalValue = totalWithdrawalCount > 0 ? totalWithdrawals / totalWithdrawalCount : 0;
            var recentWithdrawals = completedWithdrawals
                .OrderByDescending(w => w.RequestDate)
                .Take(10)
                .ToList();
            var recentWithdrawalsWithUsers = new List<RecentWithdrawalDto>();
            foreach (var withdrawal in recentWithdrawals)
            {
                string customerName = "Unknown";
                if (withdrawal.Account != null)
                {
                    customerName = withdrawal.Account.FullName ?? withdrawal.Account.UserName ?? "Unknown";
                }
                recentWithdrawalsWithUsers.Add(new RecentWithdrawalDto
                {
                    Customer = customerName,
                    Date = withdrawal.RequestDate.DateTime,
                    Amount = withdrawal.Amount,
                    BankName = withdrawal.BankName,
                    Status = withdrawal.Status.ToString()
                });
            }

            // --- Platform Service Fee statistics ---
            var allJobs = await _jobRepo.GetAllAsync();
            var jobQuery = allJobs.AsQueryable();
            if (from.HasValue)
            {
                jobQuery = jobQuery.Where(j => j.CreatedAt >= from);
            }
            if (to.HasValue)
            {
                jobQuery = jobQuery.Where(j => j.CreatedAt <= to);
            }
            var jobsWithFees = jobQuery.Where(j => j.PlatformServiceFee.HasValue && j.PlatformServiceFee > 0).ToList();
            var totalPlatformFees = jobsWithFees.Sum(j => j.PlatformServiceFee ?? 0);
            var monthlyPlatformFees = jobsWithFees
                .Where(j => j.CreatedAt.Month == DateTime.UtcNow.Month && j.CreatedAt.Year == DateTime.UtcNow.Year)
                .Sum(j => j.PlatformServiceFee ?? 0);
            var totalPlatformFeeCount = jobsWithFees.Count;
            var avgPlatformFeeValue = totalPlatformFeeCount > 0 ? totalPlatformFees / totalPlatformFeeCount : 0;
            var recentPlatformFees = jobsWithFees
                .OrderByDescending(j => j.CreatedAt)
                .Take(10)
                .ToList();
            var recentPlatformFeesWithDetails = new List<RecentPlatformFeeDto>();
            foreach (var job in recentPlatformFees)
            {
                string customerName = "Unknown";
                if (job.Customer != null)
                {
                    customerName = job.Customer.FullName ?? job.Customer.UserName ?? "Unknown";
                }
                recentPlatformFeesWithDetails.Add(new RecentPlatformFeeDto
                {
                    JobTitle = job.JobTitle,
                    Customer = customerName,
                    Date = job.CreatedAt.DateTime,
                    Amount = job.PlatformServiceFee ?? 0,
                    JobType = job.TranslationType
                });
            }

            return new RevenueDashboardDto
            {
                TotalRevenue = totalRevenue - totalWithdrawals,
                MonthlyRevenue = monthlyRevenue - monthlyWithdrawals,
                TotalTransactions = totalTransactions,
                AverageTransactionValue = avgValue,
                RevenueOverTime = revenueOverTime,
                RevenueByCategory = revenueByCategory,
                RecentTransactions = recentTransactionsWithUsers,
                TotalWithdrawals = totalWithdrawals,
                MonthlyWithdrawals = monthlyWithdrawals,
                TotalWithdrawalCount = totalWithdrawalCount,
                AverageWithdrawalValue = avgWithdrawalValue,
                RecentWithdrawals = recentWithdrawalsWithUsers,
                TotalPlatformFees = totalPlatformFees,
                MonthlyPlatformFees = monthlyPlatformFees,
                TotalPlatformFeeCount = totalPlatformFeeCount,
                AveragePlatformFeeValue = avgPlatformFeeValue,
                RecentPlatformFees = recentPlatformFeesWithDetails
            };
        }
    }
} 