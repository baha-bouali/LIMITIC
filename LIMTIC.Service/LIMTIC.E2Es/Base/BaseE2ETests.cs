using LIMTIC.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Base
{
    public class BaseE2ETests : IClassFixture<PostgresFixture>
    {
        protected readonly HttpClient Client;
        protected readonly CustomWebApplicationFactory Factory;

        public BaseE2ETests(PostgresFixture fixture)
        {
            Factory = new CustomWebApplicationFactory(fixture.ConnectionString);
            Client = Factory.CreateClient();

            // Apply migrations once
            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            db.Database.Migrate();
        }
    }
}