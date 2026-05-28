using LIMTIC.Domain.Entities.Contacts;

namespace LIMTIC.Domain.Abstractions.Contact
{
    public interface IContactRepository
    {
        Task AddAsync(ContactEntity contact);
    }
}