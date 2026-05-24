using LIMTIC.Domain.Entities.Contacts;

namespace LIMTIC.Domain.Abstractions
{
    public interface IContactRepository
    {
        Task AddAsync(ContactEntity contact);
    }
}