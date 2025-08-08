using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ITC.Repositories.Base;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ITC.API
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ITCDbContext>
    {
        public ITCDbContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var builder = new DbContextOptionsBuilder<ITCDbContext>();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlServer(connectionString);

            return new ITCDbContext(builder.Options);
        }
    }
} 