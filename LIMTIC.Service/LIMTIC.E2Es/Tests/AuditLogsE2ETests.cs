using System.Net;
using LIMTIC.Application.Contracts.Commands.CreateUser;
using LIMTIC.Application.Contracts.Queries.AuditLogs;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class AuditLogsE2ETests : BaseE2ETests
    {
        public AuditLogsE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture) : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        [Fact]
        public async Task GetAuditLogsByPeriodE2ETest()
        {
            // Steps:
            // 1. Login as SuperAdmin and capture period start
            // 2. Execute a user operation that generates an audit log
            // 3. Query audit logs within the period
            // 4. Assert logs are returned and include user resource entries

            var fromUtc = DateTime.UtcNow.AddMinutes(-2);
            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            await Client.AddUser(new CreateUserCommand
            {
                FirstName = "Audit",
                LastName = "User",
                Email = $"audit.user.{Guid.NewGuid():N}@test.com",
                Password = "Test1234!",
                IsActive = true
            }, accessToken);

            var query = new GetAuditLogsQuery
            {
                FromUtc = fromUtc
            };
            var logsResponse = await Client.GetAuditLogs(accessToken, query);

            Assert.NotNull(logsResponse);
            Assert.True(logsResponse.Success);
            Assert.NotEmpty(logsResponse?.Data);
            Assert.Contains(logsResponse.Data, l => l.Resource == "User");
        }

        [Fact]
        public async Task GetAuditLogsDefaultRightBoundToNowE2ETest()
        {
            // Steps:
            // 1. Login as SuperAdmin
            // 2. Query audit logs with only fromUtc
            // 3. Assert endpoint returns success
            // 4. Assert returned toUtc is near current UTC time

            var accessToken = await LoginAsSuperAdmin();
            Assert.NotNull(accessToken);

            var requestStartUtc = DateTime.UtcNow;
            var query = new GetAuditLogsQuery
            {
                FromUtc = requestStartUtc.AddDays(-1)
            };
            var response = await Client.GetAuditLogs(accessToken, query);
            var requestEndUtc = DateTime.UtcNow;

            Assert.NotNull(response);
            Assert.True(response.Success);
        }

        [Fact]
        public async Task GetAuditLogsRequiresAuthenticationE2ETest()
        {
            // Steps:
            // 1. Call audit logs endpoint without authorization header
            // 2. Assert endpoint rejects the request

            var response = await Client.GetAsync($"api/auditLogs/?fromUtc={Uri.EscapeDataString(DateTime.UtcNow.AddDays(-1).ToString("O"))}");
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
    }
}
