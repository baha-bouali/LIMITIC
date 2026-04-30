using FluentValidation;
using LIMTIC.Application.DTOs.Auth;

namespace LIMTIC.Application.Validations
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(e => e.Username)
                .NotEmpty().WithMessage("Username is required")
                .EmailAddress().WithMessage("Username must be a valid email address");

            RuleFor(e => e.Password)
                .NotEmpty().WithMessage("Password is required");
        }
    }
}
