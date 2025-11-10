

using FluentValidation;
using MuseumSystem.Application.Dtos.AccountDtos;

namespace MuseumSystem.Application.Validation
{
    public class AccountValidation : AbstractValidator<AccountRequestV2>
    {
        public AccountValidation() { 
        
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.")
                .MaximumLength(100).WithMessage("Full name cannot exceed 100 characters.");
            RuleFor(x => x.RoleId)
                .NotEmpty().WithMessage("Role ID is required.");
            RuleFor(x => x.MuseumId)
                .NotEmpty().WithMessage("Museum ID is required.");
        }
    }
}
