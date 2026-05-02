using FluentValidation;
using LIMTIC.Application.Commands.Login;

namespace LIMTIC.Application.Validations
{
    public class LoginCommandValidator : AbstractValidator<LoginCommand>
    {
        public LoginCommandValidator()
        {
            RuleFor(e => e.Username)
                .NotEmpty().WithMessage("Username is required")
                .EmailAddress().WithMessage("Username must be a valid email address");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
