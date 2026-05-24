using FluentValidation;
using LIMTIC.Application.DTOs.Settings;

namespace LIMTIC.Application.Validations
{
    public class UpdateSettingsValidator : AbstractValidator<UpdateSettingsDto>
    {
        public UpdateSettingsValidator()
        {
            RuleFor(x => x.Identity.LabName)
                .NotNull().NotEmpty().WithMessage("Lab name is required")
                .MaximumLength(100).WithMessage("Lab name must be at most 100 characters");

            RuleFor(x => x.Identity.ContactEmail)
                .EmailAddress().WithMessage("Contact email must be a valid email address")
                .When(x => !string.IsNullOrEmpty(x.Identity.ContactEmail));

            RuleFor(x => x.Smtp.Port)
                .InclusiveBetween(1, 65535).WithMessage("SMTP port must be between 1 and 65535");
        }
    }
}
