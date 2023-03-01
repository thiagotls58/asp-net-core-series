using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Repository;

namespace AccountOwnerServer.Configuration;

public class RepositoryContextFactory : IDesignTimeDbContextFactory<RepositoryContext>
{
    public RepositoryContext CreateDbContext(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<RepositoryContext>()
            .UseMySql(config.GetConnectionString("connection"), MySqlServerVersion.LatestSupportedServerVersion,
            opt => opt.MigrationsAssembly("Repository"));

        return new RepositoryContext(builder.Options);
    }
}
