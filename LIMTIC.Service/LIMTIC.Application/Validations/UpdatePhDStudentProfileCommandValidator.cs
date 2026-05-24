using FluentValidation;
using LIMTIC.Application.Contracts.Commands.Profiles;

namespace LIMTIC.Application.Validations
{
    public class UpdatePhDStudentProfileCommandValidator : AbstractValidator<UpdatePhDStudentProfileCommand>
    {
        public UpdatePhDStudentProfileCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId is required");

            RuleFor(x => x.EnrollmentYear)
                .GreaterThan(0).WithMessage("Enrollment year must be a positive number");
        }
    }
}
