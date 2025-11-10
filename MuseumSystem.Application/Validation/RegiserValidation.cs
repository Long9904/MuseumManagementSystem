
using FluentValidation;
using MuseumSystem.Application.Dtos.AuthDtos;

namespace MuseumSystem.Application.Validation
{
    public class RegiserValidation : AbstractValidator<RegisterRequest>
    {
        public RegiserValidation() { 
        
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.");

            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Full name is required.");

            RuleFor(x => x.MuseumName) 
                .NotEmpty().WithMessage("Museum name is required.")
                .MaximumLength(100).WithMessage("Museum name must not exceed 100 characters.");

            RuleFor(x => x.MuseumLocation)
                .NotEmpty().WithMessage("Museum location is required.")
                .MaximumLength(200).WithMessage("Museum location must not exceed 200 characters.");

            RuleFor(x => x.MuseumDescription)
                .NotEmpty().WithMessage("Museum description is required.")
                .MaximumLength(800).WithMessage("Museum description must not exceed 800 characters.");

        }
    }
}
