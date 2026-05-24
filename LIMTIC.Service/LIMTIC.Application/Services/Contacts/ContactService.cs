using FluentValidation;
using LIMTIC.Application.Abstractions.Contacts;
using LIMTIC.Application.Abstractions.Email;
using LIMTIC.Application.Contracts.Commands.Contacts;
using LIMTIC.Application.DTOs;
using LIMTIC.Application.Emails.Models;
using LIMTIC.Application.Helpers;
using LIMTIC.Domain.Abstractions;
using LIMTIC.Domain.Entities.Contacts;
using Microsoft.Extensions.Options;
using LIMTIC.Application.Settings;

namespace LIMTIC.Application.Services.Contacts
{
    public class ContactService : IContactService
    {
        private readonly IContactRepository _contactRepository;
        private readonly IEmailService _emailService;
        private readonly IValidator<SendContactMessageCommand> _validator;
        private readonly EmailSettings _emailSettings;

        public ContactService(
            IContactRepository contactRepository,
            IEmailService emailService,
            IValidator<SendContactMessageCommand> validator,
            IOptions<EmailSettings> emailSettings)
        {
            _contactRepository = contactRepository;
            _emailService = emailService;
            _validator = validator;
            _emailSettings = emailSettings.Value;
        }

        public async Task<Result<string>> SendContactMessageAsync(SendContactMessageCommand command)
        {
            var validationResult = _validator.Validate(command);
            if (!validationResult.IsValid)
                return Result<string>.ValidationFailureResult(ValidationHelper.ParseValidationErrors(validationResult));

            var sentAtUtc = DateTime.UtcNow;

            var contact = new ContactEntity
            {
                FullName = command.FullName.Trim(),
                Email = command.Email.Trim().ToLowerInvariant(),
                Subject = command.Subject.Trim(),
                Message = command.Message.Trim(),
                SentAtUtc = sentAtUtc
            };

            await _contactRepository.AddAsync(contact);

            await _emailService.SendContactEmailAsync(new ContactEmailModel
            {
                ToEmail = _emailSettings.SenderEmail,
                FullName = contact.FullName,
                SenderEmail = contact.Email,
                Subject = contact.Subject,
                Message = contact.Message,
                SentAt = sentAtUtc.ToString("yyyy-MM-dd HH:mm:ss 'UTC'")
            });

            return Result<string>.SuccessResult("Contact message sent successfully.");
        }
    }
}