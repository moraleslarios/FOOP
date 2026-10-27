// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Centraliza la construcción de los mensajes de error automáticos utilizados por <see cref="EnsureFp"/>.
/// </summary>
internal static class EnsureFpMessages
{
    internal const string DEFAULT_PARAM_NAME = "value";

    internal static string SafeName(string? paramName)
        => string.IsNullOrWhiteSpace(paramName) ? DEFAULT_PARAM_NAME : paramName!;

    internal static string NotNull(string? paramName)
        => $"'{SafeName(paramName)}' cannot be null.";

    internal static string NotEmpty(string? paramName)
        => $"'{SafeName(paramName)}' cannot be null or empty.";

    internal static string NotNullEmptyOrWhitespace(string? paramName)
        => $"'{SafeName(paramName)}' cannot be null, empty or whitespace.";

    internal static string NotValid(string? paramName)
        => $"'{SafeName(paramName)}' is not valid.";

    internal static string PredicateException(string? paramName, Exception ex)
        => $"An error occurred while validating '{SafeName(paramName)}'. Error: {ex.Message}. More info in Ex Details.";


    #region Base

    /// <summary>
    /// Construye el mensaje con el formato común: <c>'paramName' &lt;requisito&gt;.</c>
    /// </summary>
    internal static string Rule(string? paramName, string requirement)
        => $"'{SafeName(paramName)}' {requirement}.";

    private static string Actual(int? actual)
        => actual?.ToString() ?? "null";

    private static string Render(object? value)
        => value?.ToString() ?? "null";

    #endregion

    #region Strings

    internal static string MaxLength(string? paramName, int maxLength, int? actual)
        => Rule(paramName, $"must have a maximum length of {maxLength} characters (actual: {Actual(actual)})");

    internal static string MinLength(string? paramName, int minLength, int? actual)
        => Rule(paramName, $"must have a minimum length of {minLength} characters (actual: {Actual(actual)})");

    internal static string LengthBetween(string? paramName, int minLength, int maxLength, int? actual)
        => Rule(paramName, $"must have a length between {minLength} and {maxLength} characters (actual: {Actual(actual)})");

    internal static string LengthExactly(string? paramName, int length, int? actual)
        => Rule(paramName, $"must have a length of exactly {length} characters (actual: {Actual(actual)})");

    internal static string Matches(string? paramName, string pattern)
        => Rule(paramName, $"must match the pattern '{pattern}'");

    internal static string NotMatches(string? paramName, string pattern)
        => Rule(paramName, $"must not match the pattern '{pattern}'");

    internal static string StartsWith(string? paramName, string prefix)
        => Rule(paramName, $"must start with '{prefix}'");

    internal static string EndsWith(string? paramName, string suffix)
        => Rule(paramName, $"must end with '{suffix}'");

    internal static string Contains(string? paramName, string substring)
        => Rule(paramName, $"must contain '{substring}'");

    internal static string NotContains(string? paramName, string substring)
        => Rule(paramName, $"must not contain '{substring}'");

    internal static string IsOneOf(string? paramName, IEnumerable<string>? allowedValues)
        => Rule(paramName, $"must be one of the allowed values [{string.Join(", ", allowedValues ?? Enumerable.Empty<string>())}]");

    #endregion

    #region Numéricos y comparables

    internal static string GreaterThan(string? paramName, object? limit)
        => Rule(paramName, $"must be greater than {Render(limit)}");

    internal static string GreaterOrEqual(string? paramName, object? limit)
        => Rule(paramName, $"must be greater than or equal to {Render(limit)}");

    internal static string LessThan(string? paramName, object? limit)
        => Rule(paramName, $"must be less than {Render(limit)}");

    internal static string LessOrEqual(string? paramName, object? limit)
        => Rule(paramName, $"must be less than or equal to {Render(limit)}");

    internal static string InRange(string? paramName, object? min, object? max)
        => Rule(paramName, $"must be between {Render(min)} and {Render(max)}");

    internal static string OutOfRange(string? paramName, object? min, object? max)
        => Rule(paramName, $"must not be between {Render(min)} and {Render(max)}");

    internal static string Positive(string? paramName)
        => Rule(paramName, "must be greater than zero");

    internal static string NotNegative(string? paramName)
        => Rule(paramName, "cannot be negative");

    internal static string Negative(string? paramName)
        => Rule(paramName, "must be less than zero");

    internal static string NotZero(string? paramName)
        => Rule(paramName, "cannot be zero");

    #endregion

    #region Colecciones

    internal static string CountExactly(string? paramName, int expected, int? actual)
        => Rule(paramName, $"must contain exactly {expected} items (actual: {Actual(actual)})");

    internal static string CountAtLeast(string? paramName, int minCount, int? actual)
        => Rule(paramName, $"must contain at least {minCount} items (actual: {Actual(actual)})");

    internal static string CountAtMost(string? paramName, int maxCount, int? actual)
        => Rule(paramName, $"must contain at most {maxCount} items (actual: {Actual(actual)})");

    internal static string CountBetween(string? paramName, int minCount, int maxCount, int? actual)
        => Rule(paramName, $"must contain between {minCount} and {maxCount} items (actual: {Actual(actual)})");

    internal static string AllMatch(string? paramName, IEnumerable<int>? failedIndexes)
        => Rule(paramName, $"contains items that do not satisfy the condition at index [{string.Join(", ", failedIndexes ?? Enumerable.Empty<int>())}]");

    internal static string NoneMatch(string? paramName, IEnumerable<int>? matchedIndexes)
        => Rule(paramName, $"must not contain items satisfying the condition, but it does at index [{string.Join(", ", matchedIndexes ?? Enumerable.Empty<int>())}]");

    internal static string AnyMatch(string? paramName)
        => Rule(paramName, "must contain at least one item satisfying the condition");

    internal static string NoDuplicates(string? paramName)
        => Rule(paramName, "cannot contain duplicated items");

    internal static string NoNullItems(string? paramName, IEnumerable<int>? nullIndexes)
        => Rule(paramName, $"cannot contain null items, but it does at index [{string.Join(", ", nullIndexes ?? Enumerable.Empty<int>())}]");

    internal static string ContainsItem(string? paramName, object? item)
        => Rule(paramName, $"must contain the item '{Render(item)}'");

    #endregion

    #region Tipos concretos

    internal static string NotEmptyGuid(string? paramName)
        => Rule(paramName, "cannot be an empty Guid");

    internal static string IsDefinedEnum(string? paramName, Type enumType, object? value)
        => Rule(paramName, $"is not a defined value of the enum '{enumType.Name}' (actual: {Render(value)})");

    internal static string InFuture(string? paramName)
        => Rule(paramName, "must be a date in the future");

    internal static string InPast(string? paramName)
        => Rule(paramName, "must be a date in the past");

    internal static string NotDefault(string? paramName)
        => Rule(paramName, "cannot be the default value of its type");

    internal static string IsAbsoluteUri(string? paramName)
        => Rule(paramName, "must be an absolute Uri");

    internal static string IsValidUri(string? paramName)
        => Rule(paramName, "must be a valid Uri");

    internal static string IsValidEmail(string? paramName)
        => Rule(paramName, "must be a valid email address");

    internal static string FileExists(string? paramName, string? path)
        => Rule(paramName, $"must point to an existing file (path: '{Render(path)}')");

    internal static string DirectoryExists(string? paramName, string? path)
        => Rule(paramName, $"must point to an existing directory (path: '{Render(path)}')");

    #endregion
}
