using FluentValidation;
using MuseumSystem.Application.Dtos.ArtifactDtos;

namespace MuseumSystem.Application.Validation
{
    public class ArtifactValidation : AbstractValidator<ArtifactRequest>
    {
        public ArtifactValidation()
        {
            
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.");

            RuleFor(x => x.PeriodTime)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Period time is required.")
                .MaximumLength(100).WithMessage("Period time cannot exceed 100 characters.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(800).WithMessage("Description cannot exceed 800 characters.");

            RuleFor(x => x.Weight)
                .GreaterThan(-1).When(x => x.Weight.HasValue).WithMessage("Weight must be equal or greater than 0. (kg)");

            RuleFor(x => x.Height)
                .GreaterThan(-1).When(x => x.Height.HasValue).WithMessage("Height must be equal or greater than 0. (cm)");

            RuleFor(x => x.Width)
                .GreaterThan(-1).When(x => x.Width.HasValue).WithMessage("Width must be equal or greater than 0. (cm)");

            RuleFor(x => x.Length)
                .GreaterThan(-1).When(x => x.Length.HasValue).WithMessage("Length must be equal or greater than 0. (cm)");
        }
    }
}
