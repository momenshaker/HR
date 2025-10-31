using System;

namespace HR.Application.Common.Exceptions;

/// <summary>
///     Represents a pre-validation failure caused by unique constraint violations.
/// </summary>
public sealed class UniqueConstraintViolationException : Exception
{
    public UniqueConstraintViolationException(string resourceName, string field, string value)
        : base($"A {resourceName} with the specified {field} already exists.")
    {
        ResourceName = resourceName;
        Field = field;
        Value = value;
    }

    public string ResourceName { get; }

    public string Field { get; }

    public string Value { get; }
}
