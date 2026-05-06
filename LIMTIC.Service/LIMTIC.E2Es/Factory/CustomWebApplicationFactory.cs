using LIMTIC.Infrastructure.Data;
using LIMTIC.WebAPI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _connectionString;
    private readonly Dictionary<string, string?> _configOverrides;

    public CustomWebApplicationFactory(string connectionString, Dictionary<string, string?> configOverrides)
    {
        _connectionString = connectionString;
        _configOverrides = configOverrides ?? new Dictionary<string, string?>();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        builder.ConfigureAppConfiguration((context, config) =>
        {
            if (_configOverrides.Count != 0)
                config.AddInMemoryCollection(_configOverrides);
        });

        builder.ConfigureServices(services =>
        {
            // Remove existing DbContext
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));

            if (descriptor != null)
                services.Remove(descriptor);

            // Register PostgreSQL with Testcontainers
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(_connectionString));
        });
    }
}