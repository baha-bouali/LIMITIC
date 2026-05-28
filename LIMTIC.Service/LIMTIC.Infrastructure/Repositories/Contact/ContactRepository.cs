using LIMTIC.Domain.Abstractions.Contact;
using LIMTIC.Domain.Entities.Contacts;
using LIMTIC.Infrastructure.Data;

namespace LIMTIC.Infrastructure.Repositories.Contact
{
    public class ContactRepository : IContactRepository
    {
        private readonly AppDbContext _dbContext;

        public ContactRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddAsync(ContactEntity contact)
        {
            await _dbContext.Contacts.AddAsync(contact);
            await _dbContext.SaveChangesAsync();
        }
    }
}