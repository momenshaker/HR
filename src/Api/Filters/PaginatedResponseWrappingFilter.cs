using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HR.Api.Contracts;
using HR.Application.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HR.Api.Filters;

/// <summary>
///     Wraps successful responses in a <see cref="PaginatedResponse{T}" /> envelope so every endpoint shares a
///     consistent schema.
/// </summary>
public sealed class PaginatedResponseWrappingFilter : IAsyncResultFilter
{
    public Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        WrapResponse(context);
        return next();
    }

    private static void WrapResponse(ResultExecutingContext context)
    {
        if (context.Result is not ObjectResult objectResult)
        {
            return;
        }

        var value = objectResult.Value;
        if (value is null || value is ErrorResponse || value is ProblemDetails)
        {
            return;
        }

        var valueType = value.GetType();
        if (IsPaginatedResponse(valueType))
        {
            return;
        }

        var (items, elementType) = ExtractItems(value);
        if (items.Count == 0 && elementType == null)
        {
            return;
        }

        var pageNumber = GetPositiveQueryValue(context.HttpContext.Request.Query, "page") ?? 1;
        var pageSize = GetPositiveQueryValue(context.HttpContext.Request.Query, "pageSize") ?? Math.Max(items.Count, 1);
        var totalCount = Math.Max(items.Count, 0);

        var typedArray = Array.CreateInstance(elementType!, items.Count);
        for (var index = 0; index < items.Count; index++)
        {
            typedArray.SetValue(items[index], index);
        }

        var responseType = typeof(PaginatedResponse<>).MakeGenericType(elementType!);
        var collectionType = typeof(IReadOnlyCollection<>).MakeGenericType(elementType!);
        var constructor = responseType.GetConstructor(new[] { typeof(int), typeof(int), typeof(int), collectionType })!;
        var paginated = constructor.Invoke(new object[] { pageNumber, pageSize, totalCount, typedArray });

        objectResult.Value = paginated;
        objectResult.DeclaredType = responseType;
    }

    private static bool IsPaginatedResponse(Type type)
    {
        return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(PaginatedResponse<>);
    }

    private static int? GetPositiveQueryValue(IQueryCollection query, string key)
    {
        if (query.TryGetValue(key, out var values) && int.TryParse(values.FirstOrDefault(), out var parsed))
        {
            return parsed > 0 ? parsed : null;
        }

        return null;
    }

    private static (List<object?> Items, Type? ElementType) ExtractItems(object value)
    {
        if (value is string)
        {
            return (new List<object?> { value }, typeof(string));
        }

        if (value is IEnumerable enumerable)
        {
            var list = new List<object?>();
            foreach (var item in enumerable)
            {
                list.Add(item);
            }

            var elementType = DetermineElementType(value.GetType()) ?? (list.FirstOrDefault()?.GetType() ?? typeof(object));
            return (list, elementType);
        }

        return (new List<object?> { value }, value.GetType());
    }

    private static Type? DetermineElementType(Type type)
    {
        if (type.IsArray)
        {
            return type.GetElementType();
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return type.GetGenericArguments()[0];
        }

        var enumerationInterface = type.GetInterfaces()
            .FirstOrDefault(interfaceType => interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        return enumerationInterface?.GetGenericArguments()[0];
    }
}
