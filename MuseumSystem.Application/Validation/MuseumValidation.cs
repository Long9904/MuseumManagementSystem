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
        public MuseumValidation()
        {

            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop)
                .Length(1, 100).WithMessage("Name must be between 1 and 100 characters.");

            RuleFor(x => x.Location)
                 .Cascade(CascadeMode.Stop)
                .Length(1, 100).WithMessage("Location must be between 1 and 100 characters.");

            RuleFor(x => x.Description)
                    .Cascade(CascadeMode.Stop)
                .Length(1, 500).WithMessage("Description must be between 1 and 500 characters.");
        }
    }
}
