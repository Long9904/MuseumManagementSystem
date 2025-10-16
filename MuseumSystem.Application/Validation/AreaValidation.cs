using FluentValidation;
using MuseumSystem.Application.Dtos.AreaDtos;

namespace MuseumSystem.Application.Validation
{
    public class AreaValidation : AbstractValidator<AreaRequest>
    {
        public AreaValidation()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")                        
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");
        }
    }
}
