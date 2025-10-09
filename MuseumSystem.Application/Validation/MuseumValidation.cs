using FluentValidation;
using MuseumSystem.Application.Dtos.MuseumDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Validation
{
    public class MuseumValidation : AbstractValidator<MuseumRequest>
    {
        public MuseumValidation() {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Name is required.");
            RuleFor(x => x.Name.Length)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(100).WithMessage("Name cannot exceed 100 characters.")
                .GreaterThanOrEqualTo(1).WithMessage("Name must be at least 1 character long.");
            RuleFor(x => x.Location)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Location is required.");
            RuleFor(x => x.Location.Length)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(100).WithMessage("Location cannot exceed 100 characters.")
                .GreaterThanOrEqualTo(1).WithMessage("Location must be at least 1 character long.");
            RuleFor(x => x.Description)
                .Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("Description is required.");
            RuleFor(x => x.Description.Length)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(100).WithMessage("Description cannot exceed 100 characters.")
                .GreaterThanOrEqualTo(1).WithMessage("Description must be at least 1 character long.");
        }
    }
}
