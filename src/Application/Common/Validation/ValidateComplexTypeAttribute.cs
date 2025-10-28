using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Linq;

namespace HR.Application.Common.Validation;

/// <summary>
///     Lightweight replacement for ASP.NET Core's <c>ValidateComplexTypeAttribute</c> that recursively
///     validates complex properties and collections using data annotations without introducing a
///     dependency on ASP.NET in the Application layer.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public sealed class ValidateComplexTypeAttribute : ValidationAttribute
{
    /// <inheritdoc />
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is null)
        {
            return ValidationResult.Success;
        }

        var errors = value switch
        {
            IEnumerable enumerable when value is not string => ValidateEnumerable(enumerable, validationContext),
            _ => ValidateObject(value, validationContext)
        };

        return errors.Count == 0
            ? ValidationResult.Success
            : new ValidationResult(string.Join(Environment.NewLine, errors));
    }

    private static IReadOnlyCollection<string> ValidateEnumerable(IEnumerable enumerable, ValidationContext parentContext)
    {
        var errorMessages = new List<string>();
        var index = 0;

        foreach (var element in enumerable)
        {
            if (element is null)
            {
                index++;
                continue;
            }

            var elementContext = new ValidationContext(element, parentContext, parentContext.Items)
            {
                DisplayName = parentContext.DisplayName,
                MemberName = parentContext.MemberName
            };

            var elementErrors = ValidateObject(element, elementContext);
            if (elementErrors.Count > 0)
            {
                foreach (var message in elementErrors)
                {
                    errorMessages.Add($"{GetPrefixedMemberName(parentContext.MemberName, index)}: {message}");
                }
            }

            index++;
        }

        return errorMessages;
    }

    private static IReadOnlyCollection<string> ValidateObject(object value, ValidationContext context)
    {
        var validationResults = new List<ValidationResult>();
        var isValid = Validator.TryValidateObject(value, new ValidationContext(value, context, context.Items), validationResults, true);

        if (isValid)
        {
            return Array.Empty<string>();
        }

        var builder = new StringBuilder();
        var messages = new List<string>();

        foreach (var result in validationResults)
        {
            builder.Clear();

            if (result.MemberNames?.Any() == true)
            {
                builder.Append(string.Join(", ", result.MemberNames));
                builder.Append(':');
                builder.Append(' ');
            }

            builder.Append(result.ErrorMessage);
            messages.Add(builder.ToString());
        }

        return messages;
    }

    private static string GetPrefixedMemberName(string? memberName, int index)
    {
        return string.IsNullOrWhiteSpace(memberName)
            ? $"[{index}]"
            : $"{memberName}[{index}]";
    }
}
