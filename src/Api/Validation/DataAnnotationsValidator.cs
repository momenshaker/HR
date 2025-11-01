using System.ComponentModel.DataAnnotations;
using System.Linq;
using FluentValidation;
using HR.Application.Validation;

namespace HR.Api.Validation;

/// <summary>
///     Executes System.ComponentModel.DataAnnotations validation inside the FluentValidation pipeline.
/// </summary>
/// <typeparam name="T">The request type that should be validated.</typeparam>
public sealed class DataAnnotationsValidator<T> : AbstractValidator<T>
    where T : class, IValidatableRequest
{
    public DataAnnotationsValidator()
    {
        RuleFor(model => model).Custom((model, context) =>
        {
            if (model is null)
            {
                return;
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            if (!Validator.TryValidateObject(model, validationContext, validationResults, validateAllProperties: true))
            {
                foreach (var validationResult in validationResults)
                {
                    var memberNames = validationResult.MemberNames.Any()
                        ? validationResult.MemberNames
                        : new[] { context.PropertyPath };

                    foreach (var memberName in memberNames)
                    {
                        context.AddFailure(memberName, validationResult.ErrorMessage ?? "Validation failed.");
                    }
                }
            }
        });
    }
}
