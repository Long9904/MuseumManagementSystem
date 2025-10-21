using FluentValidation;
using MuseumSystem.Application.Dtos.ArtifactDtos;

namespace MuseumSystem.Application.Validation
{
    public class ArtifactMediaValidation : AbstractValidator<MediaRequest>
    {
        public ArtifactMediaValidation() { 
        
            RuleFor(x => x.File)
                .NotNull().WithMessage("Media file is required.")
                .Must(file => file != null && file.Length > 0).WithMessage("Media file cannot be empty.");
            RuleFor(x => x.Caption)
                .MaximumLength(500).WithMessage("Caption cannot exceed 500 characters.");
        }
    }
}
