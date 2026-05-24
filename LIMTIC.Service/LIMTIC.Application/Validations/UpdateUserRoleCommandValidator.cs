using FluentValidation;
using LIMTIC.Application.Contracts.Commands.UpdateUserRole;

namespace LIMTIC.Application.Validations
{
    public class UpdateUserRoleCommandValidator : AbstractValidator<UpdateUserRoleCommand>
    {
        public UpdateUserRoleCommandValidator()
        {
            RuleFor(x => x.UserId).NotEmpty().WithMessage("UserId is required");
            RuleFor(x => x.Role).IsInEnum().WithMessage("Role is invalid");
        }
    }
}
