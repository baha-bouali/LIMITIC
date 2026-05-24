using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Events;

namespace LIMTIC.Application.Validations
{
    public class UpdateSpeakerCommandValidator : AbstractValidator<UpdateSpeakerCommand>
    {
        public UpdateSpeakerCommandValidator()
        {
            RuleFor(s => s.EventId).NotEmpty();
            RuleFor(s => s.SpeakerId).NotEmpty();
            RuleFor(s => s.FirstName).NotEmpty();
            RuleFor(s => s.LastName).NotEmpty();
            RuleFor(s => s.Email).NotEmpty().EmailAddress();
            RuleFor(s => s.Institution).NotEmpty();
            RuleFor(s => s.Role).NotEmpty();
        }
    }
}
