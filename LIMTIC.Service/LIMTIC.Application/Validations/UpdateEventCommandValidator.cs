using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Events;

namespace LIMTIC.Application.Validations
{
    public class UpdateEventCommandValidator : AbstractValidator<UpdateEventCommand>
    {
        public UpdateEventCommandValidator()
        {
            RuleFor(e => e.Id).NotEmpty();
            RuleFor(e => e.Title).NotEmpty();
            RuleFor(e => e.Location).NotEmpty();
            RuleFor(e => e.Description).NotEmpty();
            RuleFor(e => e.ResearchAxisId).NotEmpty();
            RuleFor(e => e.EndDate)
                .GreaterThan(e => e.StartDate)
                .WithMessage("End date must be after start date");
        }
    }
}
