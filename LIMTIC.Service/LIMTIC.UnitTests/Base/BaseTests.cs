using LIMTIC.Application.Abstractions.UserManagement;
using LIMTIC.Application.IOC;
using LIMTIC.Application.Services.UserManagement;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Infrastructure.Data;
using LIMTIC.Infrastructure.IOC;
using LIMTIC.WebAPI.IOC;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Base
{
    public abstract class BaseTests : IDisposable
    {
        protected readonly ServiceProvider ServiceProvider;
        protected readonly AppDbContext DbContext;
        protected IUserRepository UserRepository => ServiceProvider.GetRequiredService<IUserRepository>();
        protected IUsersManagementService UsersManagementService => ServiceProvider.GetRequiredService<UsersManagementService>();
        protected IResetPasswordRepository ResetPasswordRepository => ServiceProvider.GetRequiredService<IResetPasswordRepository>();
        protected IRefreshTokenRepository RefreshTokenRepository => ServiceProvider.GetRequiredService<IRefreshTokenRepository>();

        protected BaseTests()
        {
            var services = new ServiceCollection();

            // Add configuration with empty or test settings
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection()
                .Build();

            // Register infrastructure and application modules
            services.AddInfrastructure(configuration);
            services.AddApplication();
            services.AddWebApi();

            // Remove previous AppDbContext registration if present
            var descriptor = services.FirstOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
            if (descriptor != null)
                services.Remove(descriptor);

            // Register AppDbContext with in-memory database
            services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("UnitTestDb"), ServiceLifetime.Scoped);

            ServiceProvider = services.BuildServiceProvider();
            DbContext = ServiceProvider.GetRequiredService<AppDbContext>();
        }

        public void Dispose()
        {
            DbContext?.Dispose();
            if (ServiceProvider is IDisposable disposable)
                disposable.Dispose();
        }
    }
}
