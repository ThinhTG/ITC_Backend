using ITC.BusinessObject.Entities;
using ITC.Repositories.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ITC.API.SeedData
{
    public static class SubscriptionPlanSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ITCDbContext>();
                await context.Database.MigrateAsync(); // Ensure the database is created

                if (!context.SubscriptionPlans.Any())
                {
                    await context.SubscriptionPlans.AddRangeAsync(
                        new SubscriptionPlan
                        {
                            Id = Guid.NewGuid(),
                            Name = "PartnerShip",
                            Price = 99000,
                            Description = "Increase visibility in search results and appear highlighted.",
                            DurationInDays = 30,
                            IsBoosted = true,
                            JobPostLimit = 10,
                            ServiceFeePercentage = 0.15m, // 15%
                            ApplicationLimit = 50,
                            CommissionFeePercentage = 0.08m, // 8%
                            CreatedAt = DateTime.UtcNow
                        },
                        new SubscriptionPlan
                        {
                            Id = Guid.NewGuid(),
                            Name = "Advance",
                            Price = 199000,
                            Description = "Higher priority in recommendations and more prominent display than PartnerShip.",
                            DurationInDays = 30,
                            IsBoosted = true,
                            JobPostLimit = 25,
                            ServiceFeePercentage = 0.10m, // 10%
                            ApplicationLimit = 100,
                            CommissionFeePercentage = 0.05m, // 5%
                            CreatedAt = DateTime.UtcNow
                        },
                        new SubscriptionPlan
                        {
                            Id = Guid.NewGuid(),
                            Name = "Premium",
                            Price = 299000,
                            Description = "Unlock all premium features: top visibility, AI-based suggestions, and advanced filters.",
                            DurationInDays = 30,
                            IsBoosted = true,
                            JobPostLimit = null, // Unlimited
                            ServiceFeePercentage = 0.05m, // 5%
                            ApplicationLimit = null, // Unlimited
                            CommissionFeePercentage = 0.02m, // 2%
                            CreatedAt = DateTime.UtcNow
                        }
                    );

                    await context.SaveChangesAsync();
                }
            }
        }
    }
} 