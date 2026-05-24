using FluentValidation;
using LIMTIC.Application.Contracts.Commands.ResearchAxis;

namespace LIMTIC.Application.Validations
{
    public class UpdateResearchAxisCommandValidator : AbstractValidator<UpdateResearchAxisCommand>
    {
        public UpdateResearchAxisCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id is required");
            RuleFor(x => x.Title).NotEmpty().WithMessage("Title is required");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Description is required");
        }
    }
}
