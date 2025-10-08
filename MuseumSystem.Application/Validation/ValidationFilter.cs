using FluentValidation;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MuseumSystem.Application.Validation
{
    public class ValidationFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            foreach (var arg in context.ActionArguments)
            {
                if (arg.Value == null) continue;

                var validatorType = typeof(IValidator<>).MakeGenericType(arg.Value.GetType());
                var validator = _serviceProvider.GetService(validatorType) as IValidator;
                if (validator != null)
                {
                    var result = await validator.ValidateAsync(new ValidationContext<object>(arg.Value));
                    if (!result.IsValid)
                    {
                        //var errors = result.Errors.Select(e => e.ErrorMessage).ToArray();
                        //var response = new
                        //{
                        //    statusCode = 400,
                        //    message = "Validation failed",
                        //    errors
                        //};

                        //context.Result = new BadRequestObjectResult(response);
                        //return;

                        // Middleware handle
                        throw new ValidationException(result.Errors);
                    }
                }
            }

            await next();
        }
    }
}
