// Copyright (c) 2023 Juan Francisco Morales Larios
// moraleslarios@gmail.com
// Licensed under the Apache License, Version 2.0

using System.Net.Mail;
using System.Runtime.CompilerServices;

namespace MoralesLarios.OOFP.Helpers;

/// <summary>
/// Validaciones de <see cref="EnsureFp"/> para tipos concretos habituales:
/// <see cref="Guid"/>, enumerados, fechas, <see cref="Uri"/>, correos electrónicos,
/// rutas del sistema de ficheros y nullables de tipo valor.
/// </summary>
public static partial class EnsureFp
{

    #region Guid

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> es distinto de <see cref="Guid.Empty"/>.
    /// </summary>
    public static MlResult<Guid> NotEmptyGuid(Guid value, string errorMessage)
        => That(value, value != Guid.Empty, errorMessage);

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> es distinto de <see cref="Guid.Empty"/>.
    /// </summary>
    public static MlResult<Guid> NotEmptyGuid(Guid value, MlErrorsDetails errorsDetails)
        => That(value, value != Guid.Empty, errorsDetails);

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> es distinto de <see cref="Guid.Empty"/>
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<Guid> NotEmptyGuidArg(Guid value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value != Guid.Empty, EnsureFpMessages.NotEmptyGuid(paramName), paramName);

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> nullable no es nulo ni <see cref="Guid.Empty"/>,
    /// devolviendo el valor ya desenvuelto.
    /// </summary>
    public static MlResult<Guid> NotNullNotEmptyGuid(Guid? value, string errorMessage)
        => value is not null && value.Value != Guid.Empty
                ? MlResult<Guid>.Valid(value.Value)
                : MlResult<Guid>.Fail(errorMessage);

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> nullable no es nulo ni <see cref="Guid.Empty"/>,
    /// devolviendo el valor ya desenvuelto.
    /// </summary>
    public static MlResult<Guid> NotNullNotEmptyGuid(Guid? value, MlErrorsDetails errorsDetails)
        => value is not null && value.Value != Guid.Empty
                ? MlResult<Guid>.Valid(value.Value)
                : MlResult<Guid>.Fail(errorsDetails);

    /// <summary>
    /// Comprueba que el <see cref="Guid"/> nullable no es nulo ni <see cref="Guid.Empty"/>
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<Guid> NotNullNotEmptyGuidArg(Guid? value,
                                                        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => value is not null && value.Value != Guid.Empty
                ? MlResult<Guid>.Valid(value.Value)
                : BuildRule(Guid.Empty, false, EnsureFpMessages.NotEmptyGuid(paramName), paramName);

    #endregion

    #region Enumerados

    /// <summary>
    /// Comprueba que el valor está definido en el enumerado <typeparamref name="TEnum"/>.
    /// </summary>
    public static MlResult<TEnum> IsDefined<TEnum>(TEnum value, string errorMessage)
        where TEnum : struct, Enum
        => That(value, Enum.IsDefined(typeof(TEnum), value), errorMessage);

    /// <summary>
    /// Comprueba que el valor está definido en el enumerado <typeparamref name="TEnum"/>.
    /// </summary>
    public static MlResult<TEnum> IsDefined<TEnum>(TEnum value, MlErrorsDetails errorsDetails)
        where TEnum : struct, Enum
        => That(value, Enum.IsDefined(typeof(TEnum), value), errorsDetails);

    /// <summary>
    /// Comprueba que el valor está definido en el enumerado <typeparamref name="TEnum"/>
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<TEnum> IsDefinedArg<TEnum>(TEnum value,
                                                      [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where TEnum : struct, Enum
        => BuildRule(value,
                     Enum.IsDefined(typeof(TEnum), value),
                     EnsureFpMessages.IsDefinedEnum(paramName, typeof(TEnum), value),
                     paramName);

    #endregion

    #region Fechas

    /// <summary>
    /// Comprueba que la fecha es posterior al momento actual (<see cref="DateTime.UtcNow"/> si es UTC).
    /// </summary>
    public static MlResult<DateTime> InFuture(DateTime value, string errorMessage)
        => That(value, value > NowFor(value), errorMessage);

    /// <summary>
    /// Comprueba que la fecha es posterior al momento actual.
    /// </summary>
    public static MlResult<DateTime> InFuture(DateTime value, MlErrorsDetails errorsDetails)
        => That(value, value > NowFor(value), errorsDetails);

    /// <summary>
    /// Comprueba que la fecha es posterior al momento actual generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateTime> InFutureArg(DateTime value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value > NowFor(value), EnsureFpMessages.InFuture(paramName), paramName);

    /// <summary>
    /// Comprueba que la fecha es anterior al momento actual.
    /// </summary>
    public static MlResult<DateTime> InPast(DateTime value, string errorMessage)
        => That(value, value < NowFor(value), errorMessage);

    /// <summary>
    /// Comprueba que la fecha es anterior al momento actual.
    /// </summary>
    public static MlResult<DateTime> InPast(DateTime value, MlErrorsDetails errorsDetails)
        => That(value, value < NowFor(value), errorsDetails);

    /// <summary>
    /// Comprueba que la fecha es anterior al momento actual generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateTime> InPastArg(DateTime value,
                                               [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value < NowFor(value), EnsureFpMessages.InPast(paramName), paramName);

    /// <summary>
    /// Comprueba que la fecha con desplazamiento es posterior al momento actual.
    /// </summary>
    public static MlResult<DateTimeOffset> InFuture(DateTimeOffset value, string errorMessage)
        => That(value, value > DateTimeOffset.UtcNow, errorMessage);

    /// <summary>
    /// Comprueba que la fecha con desplazamiento es posterior al momento actual generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateTimeOffset> InFutureArg(DateTimeOffset value,
                                                       [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value > DateTimeOffset.UtcNow, EnsureFpMessages.InFuture(paramName), paramName);

    /// <summary>
    /// Comprueba que la fecha con desplazamiento es anterior al momento actual.
    /// </summary>
    public static MlResult<DateTimeOffset> InPast(DateTimeOffset value, string errorMessage)
        => That(value, value < DateTimeOffset.UtcNow, errorMessage);

    /// <summary>
    /// Comprueba que la fecha con desplazamiento es anterior al momento actual generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateTimeOffset> InPastArg(DateTimeOffset value,
                                                     [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value < DateTimeOffset.UtcNow, EnsureFpMessages.InPast(paramName), paramName);

    /// <summary>
    /// Comprueba que la fecha (sin hora) es posterior al día de hoy.
    /// </summary>
    public static MlResult<DateOnly> InFuture(DateOnly value, string errorMessage)
        => That(value, value > DateOnly.FromDateTime(DateTime.Today), errorMessage);

    /// <summary>
    /// Comprueba que la fecha (sin hora) es posterior al día de hoy generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateOnly> InFutureArg(DateOnly value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value > DateOnly.FromDateTime(DateTime.Today), EnsureFpMessages.InFuture(paramName), paramName);

    /// <summary>
    /// Comprueba que la fecha (sin hora) es anterior al día de hoy.
    /// </summary>
    public static MlResult<DateOnly> InPast(DateOnly value, string errorMessage)
        => That(value, value < DateOnly.FromDateTime(DateTime.Today), errorMessage);

    /// <summary>
    /// Comprueba que la fecha (sin hora) es anterior al día de hoy generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<DateOnly> InPastArg(DateOnly value,
                                               [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value < DateOnly.FromDateTime(DateTime.Today), EnsureFpMessages.InPast(paramName), paramName);

    #endregion

    #region NotDefault

    /// <summary>
    /// Comprueba que el valor no coincide con el valor por defecto de su tipo.
    /// </summary>
    public static MlResult<T> NotDefault<T>(T value, string errorMessage)
        => That(value, ! EqualityComparer<T>.Default.Equals(value, default!), errorMessage);

    /// <summary>
    /// Comprueba que el valor no coincide con el valor por defecto de su tipo.
    /// </summary>
    public static MlResult<T> NotDefault<T>(T value, MlErrorsDetails errorsDetails)
        => That(value, ! EqualityComparer<T>.Default.Equals(value, default!), errorsDetails);

    /// <summary>
    /// Comprueba que el valor no coincide con el valor por defecto de su tipo
    /// generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> NotDefaultArg<T>(T value,
                                               [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     ! EqualityComparer<T>.Default.Equals(value, default!),
                     EnsureFpMessages.NotDefault(paramName),
                     paramName);

    #endregion

    #region Uri

    /// <summary>
    /// Comprueba que la <see cref="Uri"/> no es nula y es absoluta.
    /// </summary>
    public static MlResult<Uri> IsAbsoluteUri(Uri value, string errorMessage)
        => That(value, value is not null && value.IsAbsoluteUri, errorMessage);

    /// <summary>
    /// Comprueba que la <see cref="Uri"/> no es nula y es absoluta.
    /// </summary>
    public static MlResult<Uri> IsAbsoluteUri(Uri value, MlErrorsDetails errorsDetails)
        => That(value, value is not null && value.IsAbsoluteUri, errorsDetails);

    /// <summary>
    /// Comprueba que la <see cref="Uri"/> es absoluta generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<Uri> IsAbsoluteUriArg(Uri value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, value is not null && value.IsAbsoluteUri, EnsureFpMessages.IsAbsoluteUri(paramName), paramName);

    /// <summary>
    /// Comprueba que la cadena representa una <see cref="Uri"/> válida y devuelve la <see cref="Uri"/> construida.
    /// </summary>
    public static MlResult<Uri> IsValidUri(string value, string errorMessage, UriKind uriKind = UriKind.Absolute)
        => Uri.TryCreate(value, uriKind, out var uri)
                ? MlResult<Uri>.Valid(uri)
                : MlResult<Uri>.Fail(errorMessage);

    /// <summary>
    /// Comprueba que la cadena representa una <see cref="Uri"/> válida y devuelve la <see cref="Uri"/> construida.
    /// </summary>
    public static MlResult<Uri> IsValidUri(string value, MlErrorsDetails errorsDetails, UriKind uriKind = UriKind.Absolute)
        => Uri.TryCreate(value, uriKind, out var uri)
                ? MlResult<Uri>.Valid(uri)
                : MlResult<Uri>.Fail(errorsDetails);

    /// <summary>
    /// Comprueba que la cadena representa una <see cref="Uri"/> válida generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<Uri> IsValidUriArg(string value,
                                              UriKind uriKind = UriKind.Absolute,
                                              [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => Uri.TryCreate(value, uriKind, out var uri)
                ? MlResult<Uri>.Valid(uri)
                : BuildRule(value, false, EnsureFpMessages.IsValidUri(paramName), paramName)
                        .SecureFailErrorsDetails()
                        .ToMlResultFail<Uri>();

    #endregion

    #region Correo electrónico

    /// <summary>
    /// Comprueba que la cadena es un correo electrónico válido.
    /// La validación se realiza con <see cref="MailAddress"/>, no con expresiones regulares.
    /// </summary>
    public static MlResult<string> IsValidEmail(string value, string errorMessage)
        => That(value, IsEmail(value), errorMessage);

    /// <summary>
    /// Comprueba que la cadena es un correo electrónico válido.
    /// </summary>
    public static MlResult<string> IsValidEmail(string value, MlErrorsDetails errorsDetails)
        => That(value, IsEmail(value), errorsDetails);

    /// <summary>
    /// Comprueba que la cadena es un correo electrónico válido generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> IsValidEmailArg(string value,
                                                   [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value, IsEmail(value), EnsureFpMessages.IsValidEmail(paramName), paramName);

    #endregion

    #region Sistema de ficheros

    /// <summary>
    /// Comprueba que el fichero indicado existe.
    /// </summary>
    public static MlResult<string> FileExists(string value, string errorMessage)
        => That(value, ! string.IsNullOrWhiteSpace(value) && File.Exists(value), errorMessage);

    /// <summary>
    /// Comprueba que el fichero indicado existe.
    /// </summary>
    public static MlResult<string> FileExists(string value, MlErrorsDetails errorsDetails)
        => That(value, ! string.IsNullOrWhiteSpace(value) && File.Exists(value), errorsDetails);

    /// <summary>
    /// Comprueba que el fichero indicado existe generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> FileExistsArg(string value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     ! string.IsNullOrWhiteSpace(value) && File.Exists(value),
                     EnsureFpMessages.FileExists(paramName, value),
                     paramName);

    /// <summary>
    /// Comprueba que el directorio indicado existe.
    /// </summary>
    public static MlResult<string> DirectoryExists(string value, string errorMessage)
        => That(value, ! string.IsNullOrWhiteSpace(value) && Directory.Exists(value), errorMessage);

    /// <summary>
    /// Comprueba que el directorio indicado existe.
    /// </summary>
    public static MlResult<string> DirectoryExists(string value, MlErrorsDetails errorsDetails)
        => That(value, ! string.IsNullOrWhiteSpace(value) && Directory.Exists(value), errorsDetails);

    /// <summary>
    /// Comprueba que el directorio indicado existe generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<string> DirectoryExistsArg(string value,
                                                      [CallerArgumentExpression(nameof(value))] string? paramName = null)
        => BuildRule(value,
                     ! string.IsNullOrWhiteSpace(value) && Directory.Exists(value),
                     EnsureFpMessages.DirectoryExists(paramName, value),
                     paramName);

    #endregion

    #region Nullables de tipo valor (bloque 7)

    /// <summary>
    /// Comprueba que el nullable de tipo valor tiene valor y lo devuelve ya desenvuelto,
    /// evitando el <c>.Value</c> manual en el código llamante.
    /// </summary>
    public static MlResult<T> NotNullValue<T>(T? value, string errorMessage)
        where T : struct
        => value.HasValue
                ? MlResult<T>.Valid(value.Value)
                : MlResult<T>.Fail(errorMessage);

    /// <summary>
    /// Comprueba que el nullable de tipo valor tiene valor y lo devuelve ya desenvuelto.
    /// </summary>
    public static MlResult<T> NotNullValue<T>(T? value, MlErrorsDetails errorsDetails)
        where T : struct
        => value.HasValue
                ? MlResult<T>.Valid(value.Value)
                : MlResult<T>.Fail(errorsDetails);

    /// <summary>
    /// Comprueba que el nullable de tipo valor tiene valor generando el mensaje de error automáticamente.
    /// </summary>
    public static MlResult<T> NotNullValueArg<T>(T? value,
                                                 [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : struct
        => value.HasValue
                ? MlResult<T>.Valid(value.Value)
                : BuildRule(default(T), false, EnsureFpMessages.NotNull(paramName), paramName);

    /// <summary>
    /// Comprueba que el nullable de tipo valor tiene valor y que además cumple el predicado indicado,
    /// devolviendo el valor desenvuelto.
    /// </summary>
    public static MlResult<T> NotNullValueThat<T>(T? value, Func<T, bool> predicate, string errorMessage)
        where T : struct
        => value.HasValue && EvaluatePredicate(value.Value, predicate)
                ? MlResult<T>.Valid(value.Value)
                : MlResult<T>.Fail(errorMessage);

    #endregion

    #region Helpers privados

    private static DateTime NowFor(DateTime value)
        => value.Kind == DateTimeKind.Utc
                ? DateTime.UtcNow
                : DateTime.Now;

    private static bool IsEmail(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) return false;

        if (! MailAddress.TryCreate(value, out var mailAddress)) return false;

        return mailAddress is not null
                    && mailAddress.Address == value
                    && mailAddress.Host.Contains('.');
    }

    #endregion

}
