using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Profiles;

namespace LIMTIC.Application.Validations
{
    public class UpdateResearcherProfileCommandValidator : AbstractValidator<UpdateResearcherProfileCommand>
    {
        public UpdateResearcherProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.Rank)
                .NotNull().NotEmpty().WithMessage("Rank is required");

            RuleFor(x => x.Specialty)
                .NotNull().NotEmpty().WithMessage("Specialty is required");

            RuleFor(x => x.Office)
                .NotNull().NotEmpty().WithMessage("Office is required");

            RuleFor(x => x.PhoneNumber)
                .NotNull().NotEmpty().WithMessage("Phone number is required");
        }
    }
}
