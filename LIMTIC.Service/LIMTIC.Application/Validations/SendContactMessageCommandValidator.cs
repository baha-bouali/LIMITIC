using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Contacts;

namespace LIMTIC.Application.Validations
{
    public class SendContactMessageCommandValidator : AbstractValidator<SendContactMessageCommand>
    {
        public SendContactMessageCommandValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required")
                .MaximumLength(150).WithMessage("Full name must be at most 150 characters");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be valid")
                .MaximumLength(255).WithMessage("Email must be at most 255 characters");

            RuleFor(x => x.Subject)
                .NotEmpty().WithMessage("Subject is required")
                .MaximumLength(200).WithMessage("Subject must be at most 200 characters");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .MaximumLength(4000).WithMessage("Message must be at most 4000 characters");
        }
    }
}