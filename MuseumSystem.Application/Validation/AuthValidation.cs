using FluentValidation;
using MuseumSystem.Application.Dtos.AuthDtos;
using MuseumSystem.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MuseumSystem.Application.Validation
{
    public class AuthValidation : AbstractValidator<LoginGGRequest>
    {
        public AuthValidation()
        {
            RuleFor(x => x.IdToken).NotEmpty().WithMessage("IdToken is required.");
        }

    }
}
