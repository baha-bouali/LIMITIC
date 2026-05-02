using FluentValidation;
using LIMTIC.Application.Commands.CreateUser;

namespace LIMTIC.Application.Validations
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(e => e.FirstName)
                .NotNull().NotEmpty().WithMessage("First name is required");

            RuleFor(e => e.LastName)
                .NotNull().NotEmpty().WithMessage("Last name is required");

            RuleFor(e => e.Email)
                .NotNull().NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Email must be a valid email address");

            RuleFor(e => e.PasswordHash)
                .NotNull().NotEmpty().WithMessage("Password is required");
        }
    }
}
