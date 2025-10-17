using FluentValidation;
using MuseumSystem.Application.Dtos.RoleDtos;

namespace MuseumSystem.Application.Validation
{
    public class RoleValidation : AbstractValidator<RoleRequest>
    {
        public RoleValidation()
        {
            RuleFor(x => x.Name)
                .Cascade(CascadeMode.Stop) // Stop checking further rules if one fails                
                .Matches(@"^[a-zA-Z0-9 ]+$").WithMessage("Role name contains invalid characters.");

            RuleFor(x => x.Name.Length)
                .Cascade(CascadeMode.Stop)
                .LessThanOrEqualTo(50).WithMessage("Role name cannot exceed 50 characters.")
                .GreaterThanOrEqualTo(1).WithMessage("Role name must be at least 1 characters long.");  



        }
    }
}
