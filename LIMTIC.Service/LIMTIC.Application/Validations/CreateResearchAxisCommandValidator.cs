using FluentValidation;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;

namespace LIMTIC.Application.Validations
{
    public class CreateResearchAxisCommandValidator : AbstractValidator<CreateResearchAxisCommand>
    {
        public CreateResearchAxisCommandValidator()
        {
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        }
    }
}
