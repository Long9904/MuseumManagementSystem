using FluentValidation;
using MuseumSystem.Application.Dtos.DisplayPositionDtos;

namespace MuseumSystem.Application.Validation
{
    public class DisplayPositionValidation : AbstractValidator<DisplayPositionRequest>
    {
        public DisplayPositionValidation() { 

            RuleFor(x => x.DisplayPositionName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("DisplayPositionName is required.")
                .MaximumLength(100).WithMessage("DisplayPositionName cannot exceed 100 characters.");

            RuleFor(x => x.PositionCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("PositionCode is required.")                        
                .MaximumLength(50).WithMessage("PositionCode cannot exceed 50 characters.");

            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");    

            RuleFor(x => x.AreaId)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("AreaId is required.");
        }
    }
}
