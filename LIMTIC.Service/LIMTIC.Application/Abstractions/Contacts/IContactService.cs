using LIMTIC.Application.Contracts.Commands.Contacts;
using LIMTIC.Application.DTOs;

namespace LIMTIC.Application.Abstractions.Contacts
{
    public interface IContactService
    {
        Task<Result<string>> SendContactMessageAsync(SendContactMessageCommand command);
    }
}