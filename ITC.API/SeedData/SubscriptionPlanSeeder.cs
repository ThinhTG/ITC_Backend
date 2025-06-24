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

                var plansToSeed = new[]
                {
                    new SubscriptionPlan
                    {
                        Name = "PartnerShip", Price = 99000, Description = "Increase visibility in search results and appear highlighted.",
                        DurationInDays = 30, IsBoosted = true, JobPostLimit = 10, ServiceFeePercentage = 0.15m,
                        ApplicationLimit = 50, CommissionFeePercentage = 0.08m
                    },
                    new SubscriptionPlan
                    {
                        Name = "Advance", Price = 199000, Description = "Higher priority in recommendations and more prominent display than PartnerShip.",
                        DurationInDays = 30, IsBoosted = true, JobPostLimit = 25, ServiceFeePercentage = 0.10m,
                        ApplicationLimit = 100, CommissionFeePercentage = 0.05m
                    },
                    new SubscriptionPlan
                    {
                        Name = "Premium", Price = 299000, Description = "Unlock all premium features: top visibility, AI-based suggestions, and advanced filters.",
                        DurationInDays = 30, IsBoosted = true, JobPostLimit = null, ServiceFeePercentage = 0.05m,
                        ApplicationLimit = null, CommissionFeePercentage = 0.02m
                    }
                };

                foreach (var planData in plansToSeed)
                {
                    var existingPlan = await context.SubscriptionPlans.FirstOrDefaultAsync(p => p.Name == planData.Name);

                    if (existingPlan == null)
                    {
                        var newPlan = new SubscriptionPlan
                        {
                            Id = Guid.NewGuid(),
                            Name = planData.Name,
                            Price = planData.Price,
                            Description = planData.Description,
                            DurationInDays = planData.DurationInDays,
                            IsBoosted = planData.IsBoosted,
                            JobPostLimit = planData.JobPostLimit,
                            ServiceFeePercentage = planData.ServiceFeePercentage,
                            ApplicationLimit = planData.ApplicationLimit,
                            CommissionFeePercentage = planData.CommissionFeePercentage,
                            CreatedAt = DateTime.UtcNow
                        };
                        await context.SubscriptionPlans.AddAsync(newPlan);
                    }
                    else
                    {
                        // Plan exists, update its properties
                        existingPlan.Price = planData.Price;
                        existingPlan.Description = planData.Description;
                        existingPlan.DurationInDays = planData.DurationInDays;
                        existingPlan.IsBoosted = planData.IsBoosted;
                        existingPlan.JobPostLimit = planData.JobPostLimit;
                        existingPlan.ServiceFeePercentage = planData.ServiceFeePercentage;
                        existingPlan.ApplicationLimit = planData.ApplicationLimit;
                        existingPlan.CommissionFeePercentage = planData.CommissionFeePercentage;
                    }
                }
                await context.SaveChangesAsync();
            }
        }
    }
} 