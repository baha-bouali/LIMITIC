using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Profiles;

namespace LIMTIC.Application.Validations
{
    public class UpdateMasterianProfileCommandValidator : AbstractValidator<UpdateMasterianProfileCommand>
    {
        public UpdateMasterianProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.DissertationSubject)
                .NotNull().NotEmpty().WithMessage("Dissertation subject is required");

            RuleFor(x => x.Cohort)
                .NotNull().NotEmpty().WithMessage("Cohort is required");
        }
    }
}
