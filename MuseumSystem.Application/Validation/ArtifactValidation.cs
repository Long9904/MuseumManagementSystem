using FluentValidation;
using MuseumSystem.Application.Dtos.ArtifactDtos;

namespace MuseumSystem.Application.Validation
{
    public class ArtifactValidation : AbstractValidator<ArtifactRequest>
    {
        public ArtifactValidation()
        {
            RuleFor(x => x.ArtifactCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Artifact code is required.")
                .MaximumLength(50).WithMessage("Artifact code cannot exceed 50 characters.");

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
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.Weight)
                .GreaterThan(0).When(x => x.Weight.HasValue).WithMessage("Weight must be greater than 0 if provided. (kg)");

            RuleFor(x => x.Height)
                .GreaterThan(0).When(x => x.Height.HasValue).WithMessage("Height must be greater than 0 if provided. (cm)");

            RuleFor(x => x.Width)
                .GreaterThan(0).When(x => x.Width.HasValue).WithMessage("Width must be greater than 0 if provided. (cm)");

            RuleFor(x => x.Length)
                .GreaterThan(0).When(x => x.Length.HasValue).WithMessage("Length must be greater than 0 if provided. (cm)");
        }
    }
}
