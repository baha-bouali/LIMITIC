using LIMTIC.Domain.Abstractions.Contact;
using LIMTIC.Domain.Entities.Contacts;
using LIMTIC.Infrastructure.Repositories;
using LIMTIC.UnitTests.Base;
using Microsoft.Extensions.DependencyInjection;

namespace LIMTIC.UnitTests.Tests
{
    public class ContactRepositoryTests : BaseTests
    {
        [Fact]
        public async Task AddAsync_PersistsContact()
        {
            var sentAt = DateTime.UtcNow;
            var contact = new ContactEntity
            {
                FullName = "Ahmed Ben Ali",
                Email = "ahmed@gmail.com",
                Subject = "Information request",
                Message = "Hello, I would like more details...",
                SentAtUtc = sentAt
            };

            await ContactRepository.AddAsync(contact);
            await UnitOfWork.SaveChangesAsync();

            var saved = DbContext.Contacts.FirstOrDefault(c => c.Id == contact.Id);
            Assert.NotNull(saved);
            Assert.Equal("Ahmed Ben Ali", saved.FullName);
            Assert.Equal("Hello, I would like more details...", saved.Message);
            Assert.Equal(sentAt, saved.SentAtUtc);
            Assert.NotEqual(Guid.Empty, saved.Id);
        }
    }
}