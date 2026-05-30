using LIMTIC.Application.Contracts.Commands.Contacts;
using LIMTIC.E2Es.Base;
using LIMTIC.E2Es.Extensions;
using LIMTIC.E2Es.MailFixture;
using LIMTIC.Infrastructure.Data;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.E2Es.Tests
{
    [Collection("E2E collection")]
    public class ContactE2ETests : BaseE2ETests
    {
        public ContactE2ETests(PostgresFixture dbfixture, MailHogFixture mailFixture)
            : base(dbfixture: dbfixture, mailHogFixture: mailFixture)
        {
        }

        [Fact]
        public async Task SendContactMessage_ValidRequest_PersistsAndReturnsOk()
        {
            var request = new SendContactMessageCommand
            {
                FullName = "Ahmed Ben Ali",
                Email = "ahmed@gmail.com",
                Subject = "Information request",
                Message = "Hello, I would like more details..."
            };

            var response = await Client.SendContactMessage(request);

            Assert.True(response.Success);

            using var scope = Factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var saved = db.Contacts
                .OrderByDescending(c => c.CreatedAtUtc)
                .FirstOrDefault(c => c.Email == "ahmed@gmail.com" && c.Subject == "Information request");

            Assert.NotNull(saved);
            Assert.Equal("Ahmed Ben Ali", saved.FullName);
            Assert.Equal("Hello, I would like more details...", saved.Message);
            Assert.NotEqual(default, saved.SentAtUtc);
        }

        [Fact]
        public async Task SendContactMessage_InvalidRequest_ReturnsBadRequest()
        {
            var request = new SendContactMessageCommand
            {
                FullName = "",
                Email = "not-an-email",
                Subject = "",
                Message = ""
            };

            var response = await Client.SendContactMessage(request);

            Assert.False(response.Success);
        }
    }
}