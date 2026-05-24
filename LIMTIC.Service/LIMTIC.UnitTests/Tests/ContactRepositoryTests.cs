using LIMTIC.Domain.Entities.Contacts;
using LIMTIC.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Tests
{
    public class ContactRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddAsync_PersistsContact()
        {
            var repo = ServiceProvider.GetRequiredService<LIMTIC.Domain.Abstractions.IContactRepository>();

            var sentAt = DateTime.UtcNow;
            var contact = new ContactEntity
            {
                FullName = "Ahmed Ben Ali",
                Email = "ahmed@gmail.com",
                Subject = "Information request",
                Message = "Hello, I would like more details...",
                SentAtUtc = sentAt
            };

            await repo.AddAsync(contact);

            var saved = DbContext.Contacts.FirstOrDefault(c => c.Id == contact.Id);
            Assert.NotNull(saved);
            Assert.Equal("Ahmed Ben Ali", saved.FullName);
            Assert.Equal("Hello, I would like more details...", saved.Message);
            Assert.Equal(sentAt, saved.SentAtUtc);
            Assert.NotEqual(Guid.Empty, saved.Id);
            Assert.NotEqual(default, saved.CreatedAtUtc);
        }
    }
}